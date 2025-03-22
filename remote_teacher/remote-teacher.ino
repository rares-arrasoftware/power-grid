#include <RCSwitch.h>

RCSwitch mySwitch = RCSwitch();
int code = 0;  // Stores the computed RF code
bool transmitting = false;  // Flag to track if we're transmitting

void setup() {
    Serial.begin(9600);
    mySwitch.enableTransmit(10);  // RF Transmitter on GPIO10
    mySwitch.setPulseLength(200);
    mySwitch.setProtocol(1);
    pinMode(13, OUTPUT);
}

void loop() {
    // Check for incoming serial commands
    if (Serial.available()) {
        String command = Serial.readStringUntil('\n');  // Read command
        command.trim();  // Clean up input

        if (command.startsWith("send(")) {
            command.remove(0, 5);  // Remove "send("
            command.remove(command.length() - 1);  // Remove trailing ")"

            int commaIndex = command.indexOf(',');
            if (commaIndex == -1) {
                Serial.println("Invalid command format!");
                return;
            }

            int playerID = command.substring(0, commaIndex).toInt();
            String buttonStr = command.substring(commaIndex + 1);
            int button = 0;

            if (buttonStr == "A") button = 0b0001;
            else if (buttonStr == "B") button = 0b0010;
            else if (buttonStr == "C") button = 0b0100;
            else if (buttonStr == "D") button = 0b1000;
            else {
                Serial.println("Invalid button!");
                return;
            }

            if (playerID < 1 || playerID > 15) {
                Serial.println("Invalid player ID! (Must be 1-15)");
                return;
            }

            code = (playerID << 4) | button;  // Create 8-bit signal
            Serial.print("Starting transmission with code: ");
            Serial.println(code, BIN);
            transmitting = true;  // Enable transmission
        } 
        else if (command == "stop()") {
            Serial.println("Stopping transmission");
            transmitting = false;  // Stop sending
        } 
        else {
            Serial.println("Unknown command");
        }
    }

    // Send RF signal if transmitting
    if (transmitting) {
        digitalWrite(13, HIGH);
        for (int i = 0; i < 20; i++) {
            mySwitch.send(code, 8);  // Send 8-bit RF signal
            delay(100);
            if (!transmitting) break;  // Stop mid-transmission if requested
        }
        digitalWrite(13, LOW);
        delay(3000);
    }
}
