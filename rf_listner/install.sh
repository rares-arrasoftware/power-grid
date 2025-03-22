#!/bin/bash

set -e  # Exit on error

INSTALL_DIR="/home/power/rf_listener"
VENV_DIR="$INSTALL_DIR/venv"
SERVICE_FILE="/etc/systemd/system/rf_listener.service"

# Default values
IP="127.0.0.1"
PORT=5005
PIN=27

# Parse arguments
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --ip)
            IP="$2"
            shift 2
            ;;
        --port)
            PORT="$2"
            shift 2
            ;;
        --pin)
            PIN="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Ensure script is run as power user
if [ "$EUID" -eq 0 ]; then
    echo "Please run this script as the 'power' user, not root."
    exit 1
fi

# Create installation directory if not exists
mkdir -p $INSTALL_DIR
cd $INSTALL_DIR

# Create virtual environment
if [ ! -d "$VENV_DIR" ]; then
    echo "Creating virtual environment..."
    python -m venv venv
fi

# Activate venv and install dependencies
source $VENV_DIR/bin/activate
pip install rpi-rf
pip install rpi-lgpio

echo "Virtual environment setup complete."

# Create startup script
cat <<EOF > $INSTALL_DIR/start_rf_listener.sh
#!/bin/bash
source $VENV_DIR/bin/activate
python $INSTALL_DIR/listener.py --ip $IP --port $PORT --pin $PIN
EOF
chmod +x $INSTALL_DIR/start_rf_listener.sh

echo "Startup script created."

# Create systemd service file
sudo tee $SERVICE_FILE > /dev/null <<EOF
[Unit]
Description=RF Listener UDP Service
After=network.target

[Service]
ExecStart=$INSTALL_DIR/start_rf_listener.sh
WorkingDirectory=$INSTALL_DIR
StandardOutput=inherit
StandardError=inherit
Restart=always
User=power

[Install]
WantedBy=multi-user.target
EOF

echo "Systemd service file created."

# Enable and start service
sudo systemctl daemon-reload
sudo systemctl enable rf_listener
sudo systemctl start rf_listener

echo "Installation complete. RF Listener will start on boot with IP=$IP, PORT=$PORT, PIN=$PIN."
