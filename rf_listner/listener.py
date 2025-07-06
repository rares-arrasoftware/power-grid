import time
import socket
import signal
import argparse
import logging
import sys
import struct
from dataclasses import dataclass
from pathlib import Path
from logging.handlers import RotatingFileHandler
from rpi_rf import RFDevice
from typing import Optional, Tuple

# RF Message Format Constants (matching Arduino)
PLAYER_ID_BITS = 4
BUTTON_BITS = 4
MAX_PLAYER_ID = 15
MIN_PLAYER_ID = 1

# Button Encoding (matching Arduino)
BUTTON_A = 0b0001
BUTTON_B = 0b0010
BUTTON_C = 0b0100
BUTTON_D = 0b1000

@dataclass
class Config:
    ip: str
    port: int
    pin: int
    log_file: str
    max_log_size: int = 5 * 1024 * 1024  # 5MB
    backup_count: int = 3
    send_interval: float = 0.2  # 200ms between sends

class RFListener:
    def __init__(self, config: Config):
        self.config = config
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self._running = True
        self.last_sent_time = 0
        self.last_sent_code = None
        
        # Initialize RF device
        try:
            self.device = RFDevice(self.config.pin)
            self.device.enable_rx()
            logging.info(f"Connected to RF device on GPIO {self.config.pin}")
        except Exception as e:
            logging.error(f"Failed to initialize RF device: {e}")
            raise
        
    def stop(self) -> None:
        """Stop the listener"""
        self._running = False
        if self.device:
            self.device.cleanup()
        self.sock.close()
        
    def send_message(self, message: bytes) -> None:
        """Send message to UDP server"""
        try:
            self.sock.sendto(message, (self.config.ip, self.config.port))
            logging.info(f"Sent: {message}")
        except socket.error as e:
            logging.error(f"Failed to send message: {e}")
            
    def format_message(self, code: int) -> bytes:
        """Format RF code into UDP message"""
        message_type = 0x02  # Always 2 for remote control messages
        payload = code & 0xFF  # Ensure it's 8-bit
        return struct.pack("<II", message_type, payload)
        
    def should_send_message(self, current_time: float, code: int) -> bool:
        """Check if message should be sent based on timing and code"""
        return (current_time - self.last_sent_time >= self.config.send_interval) or (code != self.last_sent_code)
        
    def should_ignore_rf_code(self, code: int, pulselength: int, protocol: int) -> bool:
        """Return True to ignore invalid or noisy RF codes."""

        is_valid_code = code < 0xFF

        is_valid_pulse = (
            (protocol == 3 and 145 < pulselength < 165) or
            (protocol == 1 and 350 < pulselength < 370)
        )

        return not (is_valid_code and is_valid_pulse)

    def process_rf_code(self, code: int, pulselength: int, protocol: int) -> None:
        """Process received RF code and send if needed"""
        current_time = time.time()
        
        if self.should_ignore_rf_code(code, pulselength, protocol):
            return

        # Extract player ID and button from the code
        player_id = (code >> BUTTON_BITS) & 0x0F
        button = code & 0x0F
        
        # Log the received code with human-readable format
        button_str = {
            BUTTON_A: "A",
            BUTTON_B: "B",
            BUTTON_C: "C",
            BUTTON_D: "D"
        }.get(button, "Unknown")
        
        logging.info(f"Player {player_id} pressed button {button_str} [code: {code}, pulselength: {pulselength}, protocol: {protocol}]")
        
        if self.should_send_message(current_time, code):
            message = self.format_message(code)
            self.send_message(message)
            
            # Update tracking variables
            self.last_sent_time = current_time
            self.last_sent_code = code
            
    def listen(self) -> None:
        """Main listening loop"""
        timestamp = None
        while self._running:
            if self.device.rx_code_timestamp != timestamp:
                timestamp = self.device.rx_code_timestamp
                self.process_rf_code(
                    self.device.rx_code,
                    self.device.rx_pulselength,
                    self.device.rx_proto
                )
            time.sleep(0.01)

def setup_logging(config: Config) -> None:
    """Configure logging with rotation and preserve logs between runs."""
    log_dir = Path(config.log_file).parent
    log_dir.mkdir(parents=True, exist_ok=True)

    handler = RotatingFileHandler(
        config.log_file,
        maxBytes=config.max_log_size,
        backupCount=config.backup_count
    )

    formatter = logging.Formatter(
        fmt='%(asctime)-15s - [%(levelname)s] %(module)s: %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S'
    )
    handler.setFormatter(formatter)

    logger = logging.getLogger()
    logger.setLevel(logging.INFO)
    logger.addHandler(handler)

def parse_arguments() -> Config:
    """Parse command line arguments"""
    parser = argparse.ArgumentParser(description="RF Listener that sends signals to a UDP server.")
    parser.add_argument("--ip", type=str, default="127.0.0.1", help="UDP server IP address")
    parser.add_argument("--port", type=int, default=5005, help="UDP server port")
    parser.add_argument("--pin", type=int, default=27, help="GPIO pin for RF receiver")
    parser.add_argument("--log-file", type=str, default="/home/power/rf_listener/rf_listener.log",
                       help="Path to log file")
    
    args = parser.parse_args()
    return Config(
        ip=args.ip,
        port=args.port,
        pin=args.pin,
        log_file=args.log_file
    )

def main():
    config = parse_arguments()
    setup_logging(config)
    
    listener = RFListener(config)
    
    def signal_handler(signum, frame):
        logging.info(f"Received signal {signum}")
        listener.stop()
    
    # Register signal handlers
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)
    
    try:
        logging.info(f"RF Listener started. Listening on GPIO{config.pin} and sending to {config.ip}:{config.port}...")
        listener.listen()
    except KeyboardInterrupt:
        logging.info("Shutting down.")
    except Exception as e:
        logging.error(f"Unexpected error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()


