import socket
import argparse
import logging
import struct
import evdev
from dataclasses import dataclass
from typing import Optional
from pathlib import Path
import os
from logging.handlers import RotatingFileHandler

@dataclass
class Config:
    ip: str
    port: int
    device_path: str
    log_file: str
    max_log_size: int = 5 * 1024 * 1024  # 5MB
    backup_count: int = 3

class UdpClient:
    def __init__(self, ip: str, port: int):
        self.ip = ip
        self.port = port
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        
    def send_message(self, message: bytes) -> None:
        """Send message to UDP server with retry logic"""
        max_retries = 3
        for attempt in range(max_retries):
            try:
                self.sock.sendto(message, (self.ip, self.port))
                logging.info(f"Sent: {message}")
                return
            except socket.error as e:
                if attempt == max_retries - 1:
                    raise
                logging.warning(f"Failed to send message, attempt {attempt + 1}/{max_retries}: {e}")
                
    def close(self) -> None:
        self.sock.close()

class CardScanner:
    def __init__(self, device_path: str):
        self.device_path = device_path
        self.device: Optional[evdev.InputDevice] = None
        
    def connect(self) -> None:
        """Connect to the RFID scanner device"""
        try:
            self.device = evdev.InputDevice(self.device_path)
            logging.info(f"Connected to {self.device.name}")
        except Exception as e:
            logging.error(f"Failed to connect to device {self.device_path}: {e}")
            raise
            
    def read_card(self) -> Optional[int]:
        """Read a card and return its value as an integer"""
        if not self.device:
            self.connect()
            
        scanned_data = ""
        try:
            for event in self.device.read_loop():
                if event.type == evdev.ecodes.EV_KEY and event.value == 1:
                    key = evdev.categorize(event).keycode
                    
                    if key == "KEY_ENTER":
                        try:
                            return int(scanned_data) & 0xFFFFFFFF
                        except ValueError:
                            logging.error(f"Invalid RFID input: {scanned_data}")
                            return None
                    elif key.startswith("KEY_"):
                        scanned_data += key.replace("KEY_", "")
        except Exception as e:
            logging.error(f"Error reading card: {e}")
            return None

def setup_logging(config: Config) -> None:
    """Configure logging with rotation"""
    log_dir = Path(config.log_file).parent
    log_dir.mkdir(parents=True, exist_ok=True)
    
    handler = RotatingFileHandler(
        config.log_file,
        maxBytes=config.max_log_size,
        backupCount=config.backup_count
    )
    
    logging.basicConfig(
        handlers=[handler],
        level=logging.INFO,
        datefmt='%Y-%m-%d %H:%M:%S',
        format='%(asctime)-15s - [%(levelname)s] %(module)s: %(message)s'
    )

def parse_arguments() -> Config:
    """Parse command line arguments"""
    parser = argparse.ArgumentParser(description="RFID Listener that sends scanned data via UDP.")
    parser.add_argument("--ip", type=str, default="192.168.1.100", help="UDP Server IP address")
    parser.add_argument("--port", type=int, default=8081, help="UDP Server Port")
    parser.add_argument("--device", type=str, default="/dev/input/event0", help="RFID Scanner device path")
    parser.add_argument("--log-file", type=str, default="/home/power/card_scanner/card_scanner.log",
                       help="Path to log file")
    
    args = parser.parse_args()
    return Config(
        ip=args.ip,
        port=args.port,
        device_path=args.device,
        log_file=args.log_file
    )

def main():
    config = parse_arguments()
    setup_logging(config)
    
    udp_client = UdpClient(config.ip, config.port)
    card_scanner = CardScanner(config.device_path)
    
    try:
        card_scanner.connect()
        logging.info(f"Listening for RFID input from {config.device_path}, sending to {config.ip}:{config.port}")
        
        while True:
            card_value = card_scanner.read_card()
            if card_value is not None:
                message_type = 0x01
                message = struct.pack("<II", message_type, card_value)
                udp_client.send_message(message)
                
    except KeyboardInterrupt:
        logging.info("Shutting down.")
    except Exception as e:
        logging.error(f"Unexpected error: {e}")
    finally:
        udp_client.close()

if __name__ == "__main__":
    main()
