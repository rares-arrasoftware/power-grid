
# Developer Guide

## Table of Contents

1. **Overview of Layers**  
2. **Presentation Layer (WPF, MVVM)**  
3. **Application Layer (Use Cases, State Machine, Orchestrators, Timers)**  
4. **Domain Layer (Game Entities, Business Rules)**  
5. **Infrastructure Layer (Hardware Integration, Persistence, Config, Logging)**  
6. **Interaction Flow Example**  

---

## 1. Overview of Layers

```
+-----------------------------------------------------------+
|                   Presentation Layer                      |
|         (WPF, MVVM, Views, User Interaction)             |
+-----------------------------------------------------------+
                           |
                           v
+-----------------------------------------------------------+
|                     Application Layer                     |
|     (Use Cases, State Machine, Orchestrators, Timers)     |
+-----------------------------------------------------------+
                           |
                           v
+-----------------------------------------------------------+
|                       Domain Layer                        |
|      (Game Entities, Rules, Validation, Business Logic)   |
+-----------------------------------------------------------+
                           |
                           v
+-----------------------------------------------------------+
|                   Infrastructure Layer                    |
| (Raspberry Pi, UDP/TCP Communication, JSON Persistence,   |
|  RFID/Remote Adapters, Logging, Config)                   |
+-----------------------------------------------------------+
```

Each layer has clearly defined responsibilities:

- **Presentation**: Handles user interface (WPF) and user interactions (MVVM).
- **Application**: Coordinates the *game flow*, orchestrates **state machine** transitions, and invokes domain logic.
- **Domain**: Holds *business logic* and *entities* (Players, PowerPlants, etc.). No external concerns like UI or databases.
- **Infrastructure**: Provides *hardware adapters*, *networking*, *persistence*, *configuration*, and *logging*.

---

## 2. Presentation Layer (WPF, MVVM)

**Primary Responsibility**: Display the current game state to players/admins and gather user input through a graphical interface.

### 2.1 Entities / Classes

1. **Views** (XAML Pages/Windows/UserControls)  
   - **MainWindow**: The primary container for panels (Players Panel, Market Panel, etc.).  
   - **PlayersPanelView**, **MarketPanelView**, **SupplyPanelView**, **CurrentActionPanelView**, **AdminSettingsView**, etc.

2. **ViewModels**  
   - **GameViewModel**  
     - *Purpose*: Central ViewModel containing high-level game status, current phase, player list, resource market data, etc.  
     - *Key Members*:  
       - `ObservableCollection<PlayerViewModel> Players`  
       - `CurrentPhase` (bound to the state machine in the Application layer)  
       - `SelectedPowerPlant`, `SelectedSpecialCard`, etc.  
       - Commands to trigger actions (e.g., **NextPhaseCommand**, **ManualOverrideCommand**).  
   - **PlayerViewModel**  
     - *Purpose*: Represents a single player’s state (name, money, timer, resources owned, etc.).  
   - **MarketViewModel**  
     - *Purpose*: Reflects the available resources, prices, and any adjustments from event/special cards.  
   - **AdminViewModel**  
     - *Purpose*: Allows manual corrections, configuration changes (timer adjustments, reorder players, etc.).  

3. **Commands & Data Bindings**  
   - WPF uses commands (e.g., `ICommand`) to delegate user actions (button clicks) to the **Application Layer**.  
   - Bindings from ViewModels to the UI for real-time updates (player money, resource counts, phase transitions).

### 2.2 Key Responsibilities

- **Render** the game state (phases, resources, timers).  
- **Collect** user inputs from administrators (manual overrides, config changes).  
- **Display** error messages, prompts, and confirmations.  
- **Provide** a clean separation from the business logic (domain rules and application flow).

---

## 3. Application Layer (Use Cases, State Machine, Orchestrators, Timers)

**Primary Responsibility**: Coordinate the **state machine** for game flow (AuctionPowerPlants, BuyResources, BuildCities, Bureaucracy, etc.) and call *Domain* services for validation and updates.

### 3.1 Entities / Classes

1. **State Machine / PhaseManager**  
   - *Purpose*: Implements the UML state chart transitions (e.g., Auction → CardBidding → ReorderPlayers → BuildCities → Bureaucracy).  
   - *Key Methods*:  
     - `TriggerEvent(inputEvent)` - Evaluates transitions based on the current state and domain validations.  
     - `GetCurrentState()` - Returns the active state for the UI to display.  
   - *Interactions*: Subscribes to events from the Infrastructure (e.g., `ButtonPressedEvent`, `RfidCardScannedEvent`), decides next state.

2. **Use Case / Orchestrator Classes**  
   - **AuctionOrchestrator**  
     - *Purpose*: Manages the logic of auctions, from scanning power plant cards to awarding winners.  
     - Calls `AuctionService` (Domain) to validate bids, passes, etc.  
   - **ResourcePurchaseOrchestrator**  
     - *Purpose*: Handles the reverse-order purchasing of Coal, Oil, Garbage, Uranium, etc.  
     - Interacts with `ResourceService` (Domain) to ensure valid resource counts.  
   - **BuildOrchestrator**  
     - *Purpose*: Guides players through building cities, checking costs and adjacency.  
     - Calls `BuildingService` (Domain).  
   - **BureaucracyOrchestrator**  
     - *Purpose*: Final step each round—handles resource replenishment, discarding power plants, reordering players.  
     - Uses `BureaucracyService` (Domain).

3. **TimerService**  
   - *Purpose*: Manages a **chess-style** countdown per player or per phase.  
   - *Key Methods*:  
     - `StartTimer(playerId, duration)`, `PauseTimer(playerId)`, `ResumeTimer(playerId)`.  
     - `TimeExpired` event triggers auto-pass or next-phase logic in the State Machine.

### 3.2 Key Responsibilities

- **Orchestrate** domain calls (e.g., bid validation, resource purchase).  
- **Maintain** overall game flow via the state machine.  
- **Listen** to hardware/GUI events, interpret them (e.g., “Button A from Player X means a Bid or Pass in the Auction sub-state”).  
- **Coordinate** timers for each phase or player turn.

---

## 4. Domain Layer (Game Entities, Business Rules)

**Primary Responsibility**: Encapsulate all **business logic**, **validation**, and **core data models**. This is the “heart” of Power Grid’s custom rules.

### 4.1 Entities (POCOs)

1. **Player**  
   - **Properties**: `Id`, `Name`, `Money`, `PowerPlantsOwned`, `ResourcesOwned`, `HousesBuilt`, `HasPassedThisRound`, etc.  
   - **Methods**: `Bid(amount)`, `PassAuction()`, `BuyResources(resourceType, quantity)`, `BuildHouse(cityId)`, `DiscardPowerPlant(powerPlantId)`, etc.  
   - **Validation**: Checks if the player has enough money, capacity, or if it’s their turn to act.

2. **PowerPlant**  
   - **Properties**: `Id`, `ResourceType`, `Cost`, `Capacity`, `RequiredResources`.  
   - **Methods**: `CanPower(cityCount)`, `MatchesResourceType(resourceType)`, etc.

3. **ResourceMarket**  
   - **Properties**: `CoalAvailable`, `OilAvailable`, `GarbageAvailable`, `UraniumAvailable`, `CoalPrice`, etc.  
   - **Methods**: `BuyResources(player, resourceType, quantity)`, `UpdatePrices()`, `ReplenishResources(roundNumber)`.  

4. **SpecialCard** / **EventCard**  
   - **Properties**: `Id`, `Name`, `EffectDescription`, `IsOneTimeUse`, etc.  
   - **Methods**: `ApplyEffectToPlayer(Player)`, `ApplyEffectToMarket(ResourceMarket)`.  

5. **GameState**  
   - **Properties**: `CurrentPhase`, `RoundNumber`, `Players (List<Player>)`, `ResourceMarket`, `DeckPowerPlants`, `DeckSpecialCards`, etc.  
   - This is the **canonical** state used throughout the game.

### 4.2 Domain Services

1. **AuctionService**  
   - *Purpose*: Validate bids, determine winners, handle pass logic.  
   - **Methods**: `PlaceBid(player, amount)`, `ResolveAuction(...)`, `PlayerPass(...)`.  

2. **ResourceService**  
   - *Purpose*: Validate resource purchases; check storage capacity, cost, and resource availability.  
   - **Methods**: `BuyResource(player, resourceType, quantity)`.  

3. **BuildingService**  
   - *Purpose*: Validate building houses (city adjacency, cost, or any map logic if extended).  
   - **Methods**: `BuildHouse(player, cityId)`, `CalculateBuildCost(player, cityId)`.  

4. **BureaucracyService**  
   - *Purpose*: Resource replenishment, discarding power plants, reorder players.  
   - **Methods**: `ReplenishResources(gameState)`, `DiscardPowerPlant(player, powerPlantId)`, `ReorderPlayers(...)`.  

### 4.3 Key Responsibilities

- **Enforce** game constraints (money, capacity, phase restrictions).  
- **Throw exceptions** or return error results if actions violate the rules.  
- **Update** `GameState` consistently after validated actions.

---

## 5. Infrastructure Layer (Raspberry Pi, Communication, Persistence, Config)

**Primary Responsibility**: Deal with **external resources**—hardware I/O, data storage, and system-wide concerns (logging, configuration).

### 5.1 Hardware Adapters

1. **RemoteInputAdapter**  
   - *Purpose*: Receives button presses (A/B/C/D) from 433 MHz remotes via Raspberry Pi.  
   - **Methods**: `StartListening()`, `OnButtonPressed(remoteId, button)`.  
   - *Emits* events or calls **Application** to notify: `ButtonPressedEvent(playerId, buttonCode)`.

2. **RfidInputAdapter**  
   - *Purpose*: Reads RFID card UIDs from one or more readers.  
   - **Methods**: `OnCardScanned(readerId, cardUid)`.  
   - *Emits* `RfidCardScannedEvent(...)` to the Application layer.

3. **TcpUdpCommunication** (or **NetworkService**)  
   - *Purpose*: The Raspberry Pi sends events to the main Windows PC application over UDP or TCP.  
   - *Manages* connection details, message serialization/deserialization.  

### 5.2 Persistence / Data Repositories

1. **GameStateRepository**  
   - *Purpose*: Saves and retrieves the current `GameState` (e.g., after each phase) in **JSON**.  
   - **Methods**: `SaveGameState(gameState)`, `LoadGameState()`.  
   - Allows quick recovery if the system crashes or if players want to resume.

2. **ConfigurationRepository**  
   - *Purpose*: Reads game configuration from JSON (timer settings, resource prices, expansions).  
   - **Methods**: `LoadConfig()`, `SaveConfig(config)`.  

### 5.3 Logging & Error Handling

- **LoggingService**  
  - *Purpose*: Logs critical events and errors (RFID scanning errors, hardware timeouts, domain exceptions).  
  - **Methods**: `LogInfo(message)`, `LogError(exception, details)`.  

### 5.4 Key Responsibilities

- **Abstract** hardware specifics from domain logic (the game doesn’t care *how* it received a button press, just that the event occurred).  
- **Persist** game state and config in JSON or a database.  
- **Manage** communication with Raspberry Pi (for remote inputs, RFID data).  
- **Provide** system-level logging and error diagnosis.

---

## 6. Interaction Flow Example

A high-level walk-through for an **Auction** scenario:

1. **Infrastructure**: The **RfidInputAdapter** on Raspberry Pi detects a power plant card on **RFID Reader 1**. It sends a UDP message to the Windows PC:  
   `{"eventType":"CARD_SCANNED","readerId":1,"cardUid":"ABC123"}`

2. **Application (State Machine)**:  
   - Receives `CARD_SCANNED` event.  
   - Since the current state is `AuctionPowerPlants → ChoosePowerPlant`, it calls `AuctionService` in the **Domain** to verify the power plant is valid and sets up a new bidding context.

3. **Domain (AuctionService)**:  
   - Looks up the `PowerPlant` with `cardUid = "ABC123"`.  
   - Validates if it’s still available for auction.  
   - Updates `GameState` with `CurrentAuctionPlant = ABC123`.

4. **Application**:  
   - Moves the state to `CardBidding → BidOrPass`.  
   - Notifies the **TimerService** to start player clocks.  

5. **Infrastructure**: A player presses **Button A** on a 433 MHz remote. The **RemoteInputAdapter** forwards `Player3 Pressed A` to the Application.

6. **Application**:  
   - Interprets `Button A` as “I want to bid” in `BidOrPass`.  
   - Calls `AuctionService.PlaceBid(player3, amount)` (where `amount` might be set from the UI or a previous step).

7. **Domain**:  
   - Validates if `player3` has enough money.  
   - If valid, updates `GameState` with the new bid.  

8. **Presentation (WPF)**:  
   - The **GameViewModel** receives a `StateChanged` notification: “Bid from Player3.”  
   - UI highlights Player 3’s current bid, updates the timers, etc.

9. **On Auction Completion**:  
   - **Application** calls `AuctionService.ResolveAuction()`.  
   - **Domain** determines the winner, deducts money, updates `GameState`.  
   - **Presentation** refreshes to show the winner, prompting them to keep or discard a power plant.  

This sequence shows how each layer’s entities **collaborate** without leaking responsibilities across layers.

---

# Conclusion

This **Developer Guide** clarifies how each **layer** (Presentation, Application, Domain, Infrastructure) and their **entities** (ViewModels, Orchestrators, Domain Entities, Adapters, etc.) fit together.  

- **Presentation (WPF/MVVM)** handles the user interface and user actions.  
- **Application** orchestrates the *game flow*, uses a **state machine** to move between phases, and calls the **Domain** to validate and process actions.  
- **Domain** contains the heart of the *Power Grid* logic—players, power plants, resources, special cards, auctions, building, and bureaucracy.  
- **Infrastructure** deals with hardware integration (Raspberry Pi, RFID, 433 MHz remotes), data persistence (JSON), logging, and config management.

Following these guidelines ensures your Power Grid system remains **modular**, **testable**, and **extensible**—capable of handling expansions (new cards, new rules, or additional hardware) with minimal disruption.