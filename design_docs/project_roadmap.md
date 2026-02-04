# Rune Engraver - Project Roadmap

## Phase 1: Core Feedback Loop (Current Priority)
The goal of this phase is to close the loop between the visual editor and the simulation core, providing immediate feedback to the player.
- [ ] **Visual Simulation Feedback**
    - [ ] Connect `SimulationManager` signals to `RuneVisual` nodes.
    - [ ] Implement visual cues for active Qi flow (e.g., glowing wires, particle effects).
    - [ ] Visualize `QiValue` magnitude (brightness/color intensity).
- [ ] **UI & UX Improvements**
    - [ ] Improve `RunePalette` layout and categorization (Icons vs Text).
    - [ ] Enhance `PropertiesPanel` to show real-time simulation data (inspection).
    - [ ] Add basic sound effects for interaction (place, delete, connect).

## Phase 2: Game Loop & Progression
The goal of this phase is to turn the sandbox into a game with objectives.
- [ ] **Quest System & Evaluation**
    - [ ] Implement `QuestEvaluator` to check simulation outputs against win conditions.
    - [ ] Define victory criteria (e.g., "Output > 10 Fire Qi for 5 ticks").
- [ ] **Level Structure**
    - [ ] Create a Level Selection UI.
    - [ ] Design the first 5 tutorial levels (Amplifier, Combiner, Splitter, etc.).
- [ ] **Persistence**
    - [ ] Implement Save/Load for player designs (using the `GraphData` JSON format).

## Phase 3: Advanced Mechanics & Runic HDL
The goal of this phase is to introduce the unique "Coding" aspect of the game.
- [ ] **In-Game Code Editor**
    - [ ] Create a UI for typing RunicHDL code.
    - [ ] Integrate `RunicParser` to validate code in real-time.
    - [ ] Allow instantiation of "Custom Microchips" defined by player code.
- [ ] **Complex Node Implementation**
    - [ ] Expose advanced nodes (Neural Net weights, Logic Gates) in the Palette.
    - [ ] Implement `ElementFilter` and `Transmuter` visuals.

## Phase 4: Polish & Expansion
- [ ] **Worldbuilding & Metagame**
    - [ ] Integrate the "Worldbuilding Bible" lore into item descriptions and level flavor text.
    - [ ] Add a "Sect" progression system.
- [ ] **Performance Optimization**
    - [ ] Optimize the `GridSystem` for large boards.
    - [ ] Profile Simulation tick time.
