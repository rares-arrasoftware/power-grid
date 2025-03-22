import socket
import argparse
import logging
import struct
import evdev

LOG_FILE = "/home/power/card_scanner/card_scanner.log"

logging.basicConfig(filename=LOG_FILE, level=logging.INFO, datefmt='%Y-%m-%d %H:%M:%S',
                    format='%(asctime)-15s - [%(levelname)s] %(module)s: %(message)s')

# Argument parsing
def parse_arguments():
    parser = argparse.ArgumentParser(description="RFID Listener that sends scanned data via UDP.")
    parser.add_argument("--ip", type=str, default="192.168.1.100", help="UDP Server IP address")
    parser.add_argument("--port", type=int, default=8081, help="UDP Server Port")
    parser.add_argument("--device", type=str, default="/dev/input/event0", help="RFID Scanner device path")
    return parser.parse_args()

args = parse_arguments()
UDP_IP = args.ip
UDP_PORT = args.port
DEVICE_PATH = args.device

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def send_udp_message(message):
    """Send message to UDP server"""
    sock.sendto(message, (UDP_IP, UDP_PORT))
    logging.info(f"Sent: {message}")

def listen_rfid():
    """
    Listens for RFID input from the scanner device and sends it to the UDP server.
    """
    logging.info(f"Listening for RFID input from {DEVICE_PATH}, sending to {UDP_IP}:{UDP_PORT}")

    try:
        device = evdev.InputDevice(DEVICE_PATH)
        logging.info(f"Connected to {device.name}")

        scanned_data = ""
        for event in device.read_loop():
            if event.type == evdev.ecodes.EV_KEY and event.value == 1:  # Key press event
                key = evdev.categorize(event).keycode
                
                if key == "KEY_ENTER":  # RFID input is complete
                    try:
                        logging.info(f"scanned_data: {scanned_data}")
                        payload = int(scanned_data) & 0xFFFFFFFF  # Convert to 32-bit integer
                        message_type = 0x01
                        modified_code = struct.pack("<II", message_type, payload)

                        logging.info(f"Sending: {modified_code}")
                        send_udp_message(modified_code)
                        scanned_data = ""  # Reset buffer for next scan
                    except ValueError:
                        logging.error(f"Invalid RFID input: {scanned_data}")
                        scanned_data = ""  # Reset on error
                elif key.startswith("KEY_"):
                    scanned_data += key.replace("KEY_", "")
    except Exception as e:
        logging.error(f"Error: {e}")

if __name__ == "__main__":
    try:
        listen_rfid()
    except KeyboardInterrupt:
        logging.info("Shutting down.")
    finally:
        sock.close()
