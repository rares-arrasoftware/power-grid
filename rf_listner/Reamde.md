# RF Listener with UDP Integration

## Overview
This project sets up an RF listener using a **433 MHz Superheterodyne RF Receiver** connected to a **Raspberry Pi**. It listens for RF signals and sends them to a specified **UDP server**.

## Hardware Setup
We are using a **Superheterodyne RF Receiver** with four pins:

| Pin | Function | Raspberry Pi Pin |
|------|-----------|-----------------|
| 1 | GND | Pin 6 |
| 2 | Data | Pin 13 (GPIO27) |
| 3 | Data | Not connected |
| 4 | VCC (3.3V) | Pin 1 |

## Software Installation

```bash
bash install.sh --ip 127.0.0.1 --port 5005 --pin 27
```

## Usage
The listener will now start automatically when the Raspberry Pi is powered on and send RF signals to the configured UDP server.

## Uninstall
```bash
bash uninstall.sh
```
