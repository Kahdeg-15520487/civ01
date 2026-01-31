# Quest System Design: The Sect Mission Hall (宗门任务大殿)

> "The path to immortality is paved with errands for those who cannot draw a straight line."

---

## 1. Overview

The **Quest System** is the primary driver of gameplay loop. It serves as the interface between the narrative (Worldbuilding) and the gameplay (Puzzle Solving).

In-game, this is represented as the **Sect Mission Hall**, a UI hub where the player views, accepts, and submits requests.

---

## 2. Request Structure (The Puzzle Spec)

Every quest is essentially a "Work Order" defining a puzzle.

### 2.1 The "Ticket"
Each request is displayed as a Jade Slip or Scroll with:

- **Client**: Faction + Specific NPC (e.g., *Elder Mo, Azure Peak Sect*).
- **Fluff Text**: Narrative context (e.g., *"My disciples keep exploding their heating pads. Design a safer regulator."*).
- **Objective**: The puzzle goal.
- **Constraints**: Hard limits (e.g., *"Must use less than 10 Spirit Stones"*).
- **Metrics**: Optimization targets (Cost, Cycles, Size).
- **Rewards**: What you get for solving it.

### 2.2 Puzzle Definition (Technical)
A quest defines the **Input/Output** requirements for the simulation:

1.  **Input Streams**: What resources/Qi are provided?
    *   *Example*: `Stream<FireQi> [Mag: 5, Rate: 10/tick]`
2.  **Winning Conditions (Output)**: What must be produced?
    *   *Example*: `Produce 50x [Fire EssenceBall]`
    *   *Validation*: The system checks if the output matches the required Element, Magnitude, and Stability.
3.  **Test Cases**:
    *   **Standard**: Normal operation.
    *   **Edge Cases**: *"What if the input Qi spikes to Magnitude 100?"* (Corresponds to "Tribulation" or "stress testing").

---

## 3. Quest Types

### 3.1 Main Path (Dao of History)
**"Golden Thread" Quests.** These advance the plot and unlock new Tech/Chapters.
- **Source**: Azure Peak Sect (Elder Mo, Sect Leader).
- **Nature**: Unique, bespoke puzzles.
- **Unlocks**: New Rune sets, new Hardware Tiers, next Chapter.
- **Example**: *"Restore the Ancient Barrier: Requires understanding 'Looping' logic."*

### 3.2 Side Requests (Dao of Commerce)
**Optional puzzles** from other factions.
- **Source**: Flowing Cloud Valley, Obsidian Forge, etc.
- **Nature**: Variations on main mechanics. Harder constraints.
- **Rewards**: Reputation, Rare Materials, Currency (Spirit Stones).
- **Example**: *"Flowing Cloud Valley needs a 'Signal Booster' (Amplifier) that fits in a 3x3 grid."* (High space constraint).

### 3.3 Procedural/Recurring (Dao of Labor)
**Generated "Grind" tasks** for resources.
- **Source**: Generic Disciples, Commoners.
- **Nature**: Simple production tasks using existing designs. "Mass Production".
- **Rewards**: Basic resources (Spirit Stones).
- **Mechanic**: Once you solve a puzzle, you can "Automate" it. The Recurring Quest becomes a passive income stream if you assign an "Inner Disciple" (resource) to run your solution.
- **Fail State**: None. Retry indefinitely.
- **Progression**: Does not block Golden Path. Adds lore/content.

---

## 4. Faction Reputation System

Taking quests for specific factions affects standing.

| Faction | Reputation Reward | Exclusive Unlocks (Example) |
|---------|-------------------|-----------------------------|
| **Azure Peak** | **Sect Contribution** | Core Runes, Library Access |
| **Ironheart Pavilion** | **Influence** | Weapons, High-Output Emitters |
| **Flowing Cloud** | **Favor** | Teleportation Nodes, Signal Runes |
| **Obsidian Forge** | **Credit** | Rare Metals (High Conductivity) |
| **Crimson Lotus** | **Karma** | Self-Repairing Wood, Bio-Runes |

*Note: Helping Ironheart Pavilion might lower reputation with Crimson Lotus Temple.*

---

## 5. UI Concept: The Jade Board

- **The Board**: A corkboard-style view but with floating glowing Jade Slips.
- **Filters**:
  - *Urgent* (Main Story)
  - *Bounties* (Side Quests)
  - *Commissions* (Procedural)
- **Status Indicators**:
  - *In Progress*: You have accepted the slip.
  - *Verified*: Solution passes tests.
  - *Optimized*: Solution beats the "Par" metrics.

---

## 6. Example Quest: " The Temperamental Teapot"

**Client**: Outer Disciple Han (Azure Peak)
**Description**: *"I'm trying to brew Spirit Tea, but the fire keeps going out or burning the pot! I need a heater that keeps the temperature EXACTLY at Magnitude 4."*

**Input**:
- `Source`: Fire Qi (Unstable, fluctuates Mag 2-8)

**Output**:
- `Target`: Heat Emitter
- `Requirement`: Constant Mag 4 Fire Qi (+/- 0 errors allowed)

**Constraints**:
- Cost < 50
- No "Void Vents" (Don't waste the Qi, dampen it!)

**Rewards**:
- 10 Spirit Stones
- Blueprint: **Simple Thermostat** (Unlock)

---

## 7. Open Questions (Resolved)

- [x] **Failure Consequence**: **None.** Player can retry/replay indefinitely. "Must solve to proceed" for Main Story.
- [x] **Time Limits**: **None.** Optimization metrics (Cycles) exist, but no real-time countdowns.
- [x] **Rival Solutions**: **Simulated Benchmarks.** Dev-created "75% Optimum" solutions to act as the bar to beat.
  - **Rewards**: Top placement grants Achievements/Medals (Cosmetic/Prestige). No gameplay advantage to ensure fair playground.
