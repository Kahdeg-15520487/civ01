# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Rune Engraver (铭文师)** is a Zachtronics-style programming puzzle game set in a Chinese cultivation (xianxia) world, built with Godot 4.6 and C# (.NET 8). Players learn programming concepts through runic alchemy, inscribing mystical runes to power artifacts and formations.

- **Engine**: Godot 4.6 (Mono/C#)
- **Primary Languages**: GDScript (game logic, UI), C# (simulation engine)
- **Project Structure**:
  - `game/` - Main Godot project (visual programming interface, game systems)
  - `poc/` - C# simulation core, compiler, and tests
  - `MizGodotTools/` - Reusable Godot utility library (MIT licensed)
  - `design_docs/` - Game design specifications

## Development Commands

### Running the Game
```bash
# Open project in Godot (path configured in .vscode/settings.json)
"E:\Programs\Godot_v4.6-stable_mono_win64\Godot_v4.6-stable_mono_win64.exe" --path "J:\workspace2\llm\civ\game"
```

### Running C# Tests
```bash
# Run all tests
dotnet test poc/Tests/Tests.csproj

# Run tests in Docker
docker build -t runic-engraver .
docker run runic-engraver
```

### Building C# Projects
```bash
# Restore and build
dotnet build poc/RuneEngraver.Core/RuneEngraver.Core.csproj
dotnet build poc/RuneEngraver.Compiler/RuneEngraver.Compiler.csproj
```

## Architecture Overview

### Hybrid Visual + Text Programming System

The game features two complementary programming paradigms:

1. **Visual Node-Based Editor** (`EngravingTable`): Drag-and-drop runes and draw Qi traces (wires)
2. **Runic HDL** (text-based hardware description language): Can be synthesized into visual node graphs

### Core Components

#### 1. Rune System (GDScript)

Base class: `game/scripts/core/Rune.gd`

- **Rune Types**: SOURCE, PROCESSOR, STORAGE, OUTPUT
- **Key Properties**:
  - `simulation_classes`: String path to C# implementation (e.g., "Civ.Emulator.SpiritStoneSource")
  - `shape_pattern`: Array of Vector2i defining grid cells occupied
  - `socket_pattern`: Array of Vector2i defining where stones can be inserted
  - `io_definition`: Dictionary mapping port names ("in", "out") to grid positions
  - `size_in_cells`: Grid size (default 4x4 centered on origin)

#### 2. Simulation Pipeline

```
Visual Grid (EngravingTable)
    ↓ NetlistExtractor
Logical Graph (Dictionary)
    ↓ SimulationManager (C# Bridge)
RuneGraph (C# Core)
    ↓ Tick Execution
Qi Flow Simulation
```

**Key Classes**:
- `EngravingTable.gd` - Main IDE controller, handles user input, grid management, tool modes
- `NetlistExtractor.gd` - Converts visual runes+traces into logical graph representation
- `SimulationManager.cs` - Godot→C# bridge, creates RuneGraph from extracted data
- `RuneGraph.cs` - Core simulation engine with tick-based execution

#### 3. Simulation Execution (C# Core)

Location: `poc/RuneEngraver.Core/Core/Simulation/RuneGraph.cs`

Each tick executes:
1. Reset all input ports to empty
2. Transfer Qi through wires (applies decay during transit)
3. Clear output ports
4. Process nodes (calculate new outputs from inputs)
5. Check for unrouted outputs (Qi Deviation warning)

Node implementations in `poc/RuneEngraver.Core/Core/Nodes/`:
- **Sources**: `SpiritStoneSource`, `CultivatorLink`, `StoneArray`, `TunedResonator`
- **Operations**: `AmplifierNode`, `CombinerNode`, `SplitterNode`, `TransmuterNode`
- **Containers**: `QiCapacitor`, `SpiritVessel`, `DualVessel`, `ElementalPool`
- **Control**: `YinYangGate`, `ThresholdGate`, `ElementFilter`
- **Sinks**: `EffectEmitter`, `VoidDrain`, `StableEmitter`, `GroundingRod`
- **Modifiers**: `StabilizerNode`, `CatalystNode`, `CoolingChamber`

#### 4. Element System

**7 Primary Elements**: Metal, Wood, Water, Fire, Earth (5 phases) + Lightning, Wind (auxiliary)

**21 Sub-Elements**: Unstable combinations (Magma, Plasma, Steam, Mist, etc.)

Key classes:
- `QiValue.cs` - Represents elemental Qi with amplitude
- `InteractionManager.cs` - Handles element mixing, decay, and combination rules

#### 5. RunicHDL Compiler

Location: `poc/RuneEngraver.Compiler/`

Text-based hardware description language that synthesizes into visual node graphs.

**Architecture**:
- **Front-End**: `RunicParser` generates AST from `.rhdl` source files
- **Semantic Analysis**: `RunicValidator` with `SymbolTable` for type checking and validation
- **Synthesis**: `GraphBuilder` flattens AST and generates JSON graph for simulation

**Language Features**:
- `package` declarations for namespacing
- `formation` definitions (reusable node groups)
- `input`/`output` port declarations with element types and amplitude ranges
- `node` instantiation with typed parameters
- Channel connections between ports

**AST Classes** (`Syntax/Ast.cs`):
- `FormationDef`, `PortDef`, `NodeDef`, `ConnectionDef`

### Grid and Coordinate System

- `GridSystem` class handles world↔grid coordinate conversion
- Default grid: 20px cells, 200×155 grid
- Rune positions are grid-centered (Vector2i)
- Visual rendering uses world coordinates (Vector2)

### UI and Tools

**Tool Modes** (EditorToolbar):
- SELECT - Inspect runes/traces, show properties panel
- PLACE - Place runes from palette, drag stones into sockets
- ROUTE - Draw Qi traces (right-click drag, orthogonal routing)

**Panels**:
- `RunePalette` - Left sidebar, categorized rune selection
- `PropertiesPanel` - Right panel, edit rune parameters and delete entities

### Connection Logic

`NetlistExtractor` builds connectivity graph from visual traces:
1. Maps rune I/O ports to grid positions
2. Converts trace polylines into grid adjacency graph
3. Uses BFS to find connected components (nets)
4. Identifies source (output) vs target (input) ports
5. Generates directed connections for SimulationManager

## Programming Concepts ↔ Xianxia Mapping

| Programming | Xianxia | Implementation |
|------------|---------|----------------|
| Variables | Spirit Containers (灵器) | `QiCapacitor`, `SpiritVessel` nodes |
| Functions | Rune Patterns (符文阵) | Formations (reusable node groups) |
| Conditionals | Yin-Yang Gates (阴阳门) | `YinYangGate` routes by amplitude |
| Loops | Circuit Topology | Feedback loops via trace routing |
| Data Types | 7 Elements | `ElementType` enum in C# |
| Composition | Formation Nesting (阵中阵) | Formations within formations |

## Important Conventions

- **GDScript**: snake_case files, PascalCase classes
- **C#**: PascalCase for everything
- **Rune IDs**: lowercase with underscores (e.g., `source_socket`, `item_stone_fire`)
- **Simulation Class Names**: PascalCase, map 1:1 with C# node types
- **Grid Coordinates**: Vector2i (integers), world coordinates: Vector2 (floats)

### Socket System

Runes can have "sockets" (hollow cells) where elemental stones are inserted:
- Drag stone items from palette onto socket arrays
- `socket_pattern` defines valid socket positions relative to rune center
- Parameters stored in `entry["params"]` dictionary:
  - `"stone_type"` - Single socket element
  - `"element_slots"` - Array of socket elements

## File Structure Notes

- `game/scenes/` - Godot scene files (.tscn)
- `game/scripts/core/` - Base systems (Rune, GridSystem)
- `game/scripts/systems/` - Game systems (EngravingTable, RuneVisual, properties)
- `game/scripts/ui/` - UI components
- `game/scripts/emulator/` - C# bridge (`SimulationManager.cs`)
- `poc/RuneEngraver.Core/` - Pure C# simulation (no Godot dependencies)
- `poc/RuneEngraver.Compiler/` - RunicHDL parser and compiler
- `poc/Tests/` - xUnit tests for simulation and compiler
