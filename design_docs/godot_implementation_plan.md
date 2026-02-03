# Implementation Plan - Rune Engraver (铭文师)

This plan outlines the technical steps to build the vertical slice (Mortal Tier prototype) of *Rune Engraver*.

## User Review Required

> [!IMPORTANT]
> **Engine Version**: Godot 4.3 (Latest Stable)
> **Language**: GDScript (Strict typing recommended)
> **Grid System**: TileMapLayer vs Custom Grid?
> *Decision*: Custom Grid Resource based on 2D Arrays for easier simulation logic, using TileMap only for rendering.

## Proposed Changes

### 1. Project Setup
#### [NEW] [project.godot](file:///project.godot)
- Initialize Godot 4 project.
- Configure display resolution (1920x1080).
- Set up input map (Left Click, Right Click, Drag, Zoom).
- Install **GodotSteam** plugin (placeholder for now).

#### [NEW] [Folder Structure]
```
res://
├── assets/         # Sprites, Fonts, Audio
├── src/
│   ├── core/       # Simulation logic (Pure Data)
│   ├── systems/    # Autoloads (QuestManager, SaveSystem)
│   ├── ui/         # HUD, Editor, Tooltips
│   └── nodes/      # In-game objects (RuneNode, Wire)
├── scenes/         # MainMenu, Editor, World
├── tests/          # Unit tests for logic
└── data/           # JSON/Resource definitions (Elements, Nodes)
```

### 2. Core Simulation (The "Backend")
#### [NEW] [SimulationEngine.gd](file:///src/core/simulation_engine.gd)
- `tick()` function: The heartbeat of the game.
- Phase 1: **Propagation** (Move Qi along wires).
- Phase 2: **Processing** (Nodes consume inputs, produce outputs).
- Phase 3: **Decay** (Sub-elements lose TTL).

#### [NEW] [RuneGrid.gd](file:///src/core/rune_grid.gd)
- Data structure representing the artifact's circuit board.
- `Dictionary` based sparse grid or 2D Array `[x][y]`.
- Handles connection validation.
- **Support for Multi-Tile Nodes**: `occupies` list of coordinates. A 2x2 node occupies 4 cells but is a single logical entity.

#### [NEW] [Element.gd](file:///src/core/element.gd)
- Class defining `type` (Enum), `magnitude` (int), `ttl` (int).
- Helper functions: `combine(other)`, `transmute()`.

### 3. Editor UI (The "Frontend")
#### [NEW] [EditorScene.tscn](file:///scenes/editor/editor_scene.tscn)
- **Palette**: Drag-and-drop nodes.
- **Grid View**: Visual representation of the `RuneGrid`.
- **Inspector**: View details of selected node/wire.
- **Simulation Controls**: Play, Pause, Step, adjusted Tick Rate.

### 4. Persistence
#### [NEW] [SaveSystem.gd](file:///src/systems/save_system.gd)
- Serializes `RuneGrid` to JSON.
- Format:
  ```json
  {
    "grid_size": [10, 10],
    "nodes": [
      { "type": "fire_source", "pos": [2, 3] },
      { "type": "amplifier", "pos": [4, 3] }
    ],
    "wires": [
      { "from": [2, 3], "to": [4, 3], "port": 0 }
    ]
  }
  ```

## Verification Plan

### Automated Tests
- **Unit Tests**:
  - Test `Element.combine()` logic against 7-element spec.
  - Test `Amplifier` node logic (input 5 Fire + 2 Earth -> 10 Earth).
  - Test `Circuit` topology (loops, branches).

### Manual Verification
- **Visual Test**: Drag a "Fire Source" and connect to "Visual Emitter".
- **Step Test**: Click "Step" button and verify Qi moves 1 tile per click.
- **Stress Test**: Create an infinite loop of `Overrun Emitters` to see if game crashes or handles numbers gracefully (Cap at 999 or explode?).
