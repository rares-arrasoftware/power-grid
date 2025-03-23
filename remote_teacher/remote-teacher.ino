#include <RCSwitch.h>

// RF Configuration
const int RF_PIN = 10;        // RF Transmitter on GPIO10
const int LED_PIN = 13;       // Built-in LED for visual feedback
const int PULSE_LENGTH = 200; // RF pulse length
const int PROTOCOL = 1;       // RF protocol number
const int BAUD_RATE = 9600;   // Serial communication speed

// Message Format Constants
const int PLAYER_ID_BITS = 4;     // Number of bits for player ID
const int BUTTON_BITS = 4;        // Number of bits for button
const int MAX_PLAYER_ID = 15;     // Maximum player ID (4 bits)
const int MIN_PLAYER_ID = 1;      // Minimum player ID

// Button Encoding
const int BUTTON_A = 0b0001;
const int BUTTON_B = 0b0010;
const int BUTTON_C = 0b0100;
const int BUTTON_D = 0b1000;

// Transmission Settings
const int TRANSMISSION_COUNT = 20;  // Number of times to repeat the signal
const int TRANSMISSION_DELAY = 100; // Delay between transmissions (ms)
const int COOLDOWN_DELAY = 3000;    // Delay after transmission (ms)
const int SERIAL_TIMEOUT = 1000;    // Serial timeout in ms

// Error States
const int ERROR_NONE = 0;
const int ERROR_INVALID_COMMAND = 1;
const int ERROR_INVALID_BUTTON = 2;
const int ERROR_INVALID_PLAYER = 3;
const int ERROR_SERIAL = 4;

RCSwitch mySwitch = RCSwitch();
int code = 0;  // Stores the computed RF code
bool transmitting = false;  // Flag to track if we're transmitting
int lastError = ERROR_NONE;  // Track last error state

void setup() {
    Serial.begin(BAUD_RATE);
    Serial.setTimeout(SERIAL_TIMEOUT);  // Set serial timeout
    
    // Configure RF transmitter
    mySwitch.enableTransmit(RF_PIN);
    mySwitch.setPulseLength(PULSE_LENGTH);
    mySwitch.setProtocol(PROTOCOL);
    
    // Configure LED
    pinMode(LED_PIN, OUTPUT);
    
    // Initial state
    digitalWrite(LED_PIN, LOW);
}

int parseButton(String buttonStr) {
    char c = buttonStr[0] & 0xDF;  // Convert to uppercase with bitmask
    if (c >= 'A' && c <= 'D') {
        return 1 << (c - 'A');  // Shift based on position from A
    }
    return -1;
}

bool validatePlayerId(int playerId) {
    return playerId >= MIN_PLAYER_ID && playerId <= MAX_PLAYER_ID;
}

void sendRFCode() {
    digitalWrite(LED_PIN, HIGH);
    
    for (int i = 0; i < TRANSMISSION_COUNT && transmitting; i++) {
        mySwitch.send(code, PLAYER_ID_BITS + BUTTON_BITS);
        delay(TRANSMISSION_DELAY);
    }
    
    digitalWrite(LED_PIN, LOW);
    delay(COOLDOWN_DELAY);
}

void reportError(int error) {
    lastError = error;
    Serial.print("ERROR:");
    switch(error) {
        case ERROR_INVALID_COMMAND:
            Serial.println("Invalid command format");
            break;
        case ERROR_INVALID_BUTTON:
            Serial.println("Invalid button (use A, B, C, or D)");
            break;
        case ERROR_INVALID_PLAYER:
            Serial.println("Invalid player ID (must be 1-15)");
            break;
        case ERROR_SERIAL:
            Serial.println("Serial communication error");
            break;
        default:
            Serial.println("Unknown error");
    }
}

void processCommand(String command) {
    // Expected format: "X,Y" where X is player ID and Y is button (A-D)
    int commaIndex = command.indexOf(',');
    if (commaIndex == -1) {
        reportError(ERROR_INVALID_COMMAND);
        return;
    }

    int playerId = command.substring(0, commaIndex).toInt();
    int button = parseButton(command.substring(commaIndex + 1));

    if (button == -1 || !validatePlayerId(playerId)) {
        reportError(button == -1 ? ERROR_INVALID_BUTTON : ERROR_INVALID_PLAYER);
        return;
    }

    code = (playerId << BUTTON_BITS) | button;
    Serial.print("TX: ");
    Serial.println(code, BIN);
    transmitting = true;
}

void loop() {
    if (Serial.available()) {
        String command = Serial.readStringUntil('\n');
        command.trim();
        
        if (command == "stop") {
            Serial.println("STOP");
            transmitting = false;
            return;
        }
        
        processCommand(command);
    }

    if (transmitting) {
        sendRFCode();
    }
    
    if (!transmitting && !Serial.available()) {
        delay(10);
    }
}
