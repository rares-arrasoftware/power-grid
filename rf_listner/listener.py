import time
import socket
import signal
import argparse
import logging
import sys
import struct

from rpi_rf import RFDevice

rfdevice = None

LOG_FILE = "/home/power/rf_listener/rf_listener.log"

logging.basicConfig(filename=LOG_FILE,level=logging.INFO, datefmt='%Y-%m-%d %H:%M:%S',
                    format='%(asctime)-15s - [%(levelname)s] %(module)s: %(message)s', )

# pylint: disable=unused-argument
def exithandler(signal, frame):
    rfdevice.cleanup()
    sys.exit(0)

# Argument parsing
def parse_arguments():
    parser = argparse.ArgumentParser(description="RF Listener that sends signals to a UDP server.")
    parser.add_argument("--ip", type=str, default="127.0.0.1", help="UDP server IP address")
    parser.add_argument("--port", type=int, default=5005, help="UDP server port")
    parser.add_argument("--pin", type=int, default=27, help="GPIO pin for RF receiver (default: GPIO27 / Pin 13 on Raspberry Pi)")
    return parser.parse_args()

# Parse arguments
args = parse_arguments()
UDP_IP = args.ip
UDP_PORT = args.port
RF_PIN = args.pin

# Create UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def send_udp_message(message):
    """Send message to UDP server"""
    sock.sendto(message, (UDP_IP, UDP_PORT))
    print(f"Sent: {message}")
    logging.info(f"Sent: {message}")


def listen_rf():
    """Listen for RF signals and send them via UDP"""
    signal.signal(signal.SIGINT, exithandler)
    rfdevice = RFDevice(args.pin)
    rfdevice.enable_rx()
    timestamp = None
    logging.info("Listening for codes on GPIO " + str(args.pin))

    last_sent_time = 0  # Timestamp of the last sent message
    send_interval = 0.2  # 200ms (0.2 seconds)

    timestamp = None  # Track last received timestamp
    last_sent_code = None  # Track last sent RF code

    while True:
        if rfdevice.rx_code_timestamp != timestamp:
            timestamp = rfdevice.rx_code_timestamp
            current_time = time.time()  # Get current time in seconds

            logging.info(f"{rfdevice.rx_code} [pulselength {rfdevice.rx_pulselength}, protocol {rfdevice.rx_proto}]")

            # Check if enough time has passed AND if the code is different
            if (current_time - last_sent_time >= send_interval) or (rfdevice.rx_code != last_sent_code):
                message_type = 0x02  # Always 2
                payload = rfdevice.rx_code & 0xFF  # Ensure it's 8-bit

                # Pack the values into an 8-byte structure (2 x 4-byte integers)
                modified_code = struct.pack("<II", message_type, payload)
                logging.info(f"Sending: {modified_code}")

                send_udp_message(modified_code)  # Send the message

                # Update tracking variables
                last_sent_time = current_time
                last_sent_code = rfdevice.rx_code

        time.sleep(0.01)
    rfdevice.cleanup()

if __name__ == "__main__":
    try:
        print(f"RF Listener started. Listening on GPIO{RF_PIN} and sending to {UDP_IP}:{UDP_PORT}...")
        logging.info(f"RF Listener started. Listening on GPIO{RF_PIN} and sending to {UDP_IP}:{UDP_PORT}...")
        listen_rf()
    except KeyboardInterrupt:
        logging.info("Shutting down.")


