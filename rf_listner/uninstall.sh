#!/bin/bash

set -e  # Exit on error

INSTALL_DIR="/home/power/rf_listener"
SERVICE_FILE="/etc/systemd/system/rf_listener.service"

# Ensure script is run as pi user
if [ "$EUID" -eq 0 ]; then
    echo "Please run this script as the 'pi' user, not root."
    exit 1
fi

# Stop and disable service
if sudo systemctl is-active --quiet rf_listener; then
    echo "Stopping RF Listener service..."
    sudo systemctl stop rf_listener
fi

echo "Disabling RF Listener service..."
sudo systemctl disable rf_listener

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

echo "Uninstallation complete. RF Listener has been removed."
