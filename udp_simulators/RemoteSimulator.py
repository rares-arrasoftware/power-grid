import socket
import struct
import asyncio

class Client:
    def __init__(self, server_ip: str, server_port: int):
        self.server_address = (server_ip, server_port)
        self.udp_client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    async def send_data(self, player_id: int, button: str):
        button = button.upper()

        if button not in 'ABCD':
            print("Invalid button input. Please use A, B, C, or D.")
            return

        if player_id > 0x0F:
            print("Player ID must be a 4-bit value (0-15).")
            return

        msg = 0x02
        button_value = ord(button) - ord('A')
        payload = (player_id << 4) | (1 << button_value)

        # Convert to 8-byte message (2 integers)
        message_bytes = struct.pack('ii', msg, payload)

        self.udp_client.sendto(message_bytes, self.server_address)
        print(f"Sent data: Player {player_id}, Button {button}")

    def close(self):
        self.udp_client.close()
        print("Client closed.")

async def main():
    client = Client("127.0.0.1", 8081)  # Replace with actual server IP and port

    try:
        while True:
            player_id = int(input("Enter player ID (0-15): "))
            button = input("Enter button (A, B, C, D): ")
            await client.send_data(player_id, button)
    except KeyboardInterrupt:
        print("\nExiting...")
    finally:
        client.close()

if __name__ == "__main__":
    asyncio.run(main())
