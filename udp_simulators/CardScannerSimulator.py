import socket
import struct
import asyncio

class CardScannerSimulator:
    def __init__(self, server_ip: str, server_port: int):
        self.server_address = (server_ip, server_port)
        self.udp_client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    async def send_card_scan(self, card_id: int):
        if not (0 <= card_id <= 0xFFFFFFFF):
            print("Card ID must be a 32-bit integer (0-4294967295).")
            return

        msg = 0x01  # Message Type: Card Scan
        payload = card_id  # The card ID is sent as the second integer

        # Convert to 8-byte message (2 integers)
        message_bytes = struct.pack('ii', msg, payload)

        self.udp_client.sendto(message_bytes, self.server_address)
        print(f"Sent data: Card Scanned - ID {card_id}")

    def close(self):
        self.udp_client.close()
        print("CardScannerSimulator closed.")

async def main():
    scanner = CardScannerSimulator("127.0.0.1", 8081)  # Replace with actual server IP and port

    try:
        while True:
            card_id = int(input("Enter Card ID (0-4294967295): "))
            await scanner.send_card_scan(card_id)
    except KeyboardInterrupt:
        print("\nExiting...")
    finally:
        scanner.close()

if __name__ == "__main__":
    asyncio.run(main())
