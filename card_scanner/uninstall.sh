#!/bin/bash

set -e  # Exit on error

INSTALL_DIR="/home/power/card_scanner"
SERVICE_FILE="/etc/systemd/system/card_scanner.service"

# Ensure script is run as power user
if [ "$EUID" -eq 0 ]; then
    echo "Please run this script as the 'power' user, not root."
    exit 1
fi

# Stop and disable service
if sudo systemctl is-active --quiet card_scanner; then
    echo "Stopping Card Scanner service..."
    sudo systemctl stop card_scanner
fi

echo "Disabling Card Scanner service..."
sudo systemctl disable card_scanner

# Remove service file
if [ -f "$SERVICE_FILE" ]; then
    echo "Removing service file..."
    sudo rm "$SERVICE_FILE"
    sudo systemctl daemon-reload
fi

# Remove installation directory
if [ -d "$INSTALL_DIR" ]; then
    echo "Removing installation directory..."
    rm -rf "$INSTALL_DIR"
fi

echo "Uninstallation complete. Card Scanner has been removed."
