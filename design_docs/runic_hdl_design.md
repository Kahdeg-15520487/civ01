# RunicHDL Design Specification

## 1. Introduction

RunicHDL is a hardware description language designed to describe Runic Circuits as defined in the Runic Language Specification. It allows for the textual representation of formations, nodes, and Qi channels, enabling synthesis into the visual node-based system.

## 2. Core Concepts

*   **Formation**: The top-level unit.
*   **Package**: A namespace containing multiple definitions (e.g., `std.fire`).
*   **Node**: An instance of a standard Rune or a Formation.
*   **Channel**: A connection between ports.

## 3. Syntax Structure

### 3.1 Formation Definition

```runic
// Define the package this file belongs to
package runic.examples;

// Import specific formation
import std.fire.FireStarter;
// Import all from package
import std.earth.*;

formation <Name> {
    // ...
}
```

### 3.2 Ports

Ports define the interface of a formation.
When a formation is used as a node, these become its input/output ports.

```runic
input <Element> <Name> [ <AmplitudeRange> ];
output <Element> <Name>;
```

*   `Element`: Wood, Fire, Earth, Metal, Water, Lightning, Wind, Any.
*   `AmplitudeRange`: `[min..max]`, `[min+]`, or `[exact]`.

**Example:**
```runic
// 'ignition' is the name of the input port
// 'magma_flow' is the name of the output port
input Fire ignition [5+];
output Earth magma_flow;
```

### 3.3 Node Instantiation

Nodes are instances of built-in primitives or other formations.

```runic
node <Type> <InstanceName> ( <Parameters> );
```

**Semantics:**
*   **Keywords**: `formation`, `node`, `input`, `output` are reserved.
*   **Built-in Types**: `Wood`, `Fire`, `SpiritStoneSocket`, etc., are defined in the Standard Runic Library.
*   **Identifiers**: `ignition`, `t1`, `gate1` are user-defined names for this specific scope.

**Parameters:**
Parameters are typed key-value pairs passed to the node. The compiler validates these against the node's definition.
*   **Types supported**:
    *   `String`: `"Fireball"`
    *   `Integer`: `50`
    *   `Float`: `0.5`
    *   `Boolean`: `true`
    *   `Enum/Identifier`: `Medium`, `Fire` (treated as specific constants)
*   **Handling**: Nodes "pull" parameters by name. Unknown parameters are ignored (with a warning).

**Verified Built-in Types (Ref: `runic_language_spec.md`):**
*   `SpiritStoneSocket` (Sec 4.1)
*   `QiCapacitor`, `BurstTrigger` (Sec 4.1)
*   `Transmuter`, `Amplifier`, `Dampener`, `Splitter`, `Combiner` (Sec 4.4)
*   `YinYangGate`, `ThresholdGate`, `ElementFilter` (Sec 4.5)
*   `EffectEmitter` (Sec 4.2)
*   `VoidDrain` (Sec 4.2)

> **Note**: Any node not listed in the Spec must be defined as a `Formation` or added to the spec before use.

**Using Custom Formations:**

```runic
import user.utils.FireStarter;

node FireStarter my_starter;
```

## 4. Package System

RunicHDL uses a package system to organize code.

### 4.1 Declaration
Every file must declare its package at the top.
```runic
package <Namespace>;
```

### 4.2 Imports
```runic
import <Package>.<Member>;
import <Package>.*;
```

### 4.3 Resolution Rules
*   **`std.*`**: Mapped to the compiler's standard library.
*   **`core.*`**: Native built-in nodes (implicitly imported).
*   **Other**: Mapped to the file system relative to the project root.
    *   `import clans.sect_a.defenses;` -> looks for `clans/sect_a/defenses.rhdl` or `clans/sect_a/defenses/*.rhdl`.

### 4.4 Dependency Management (Remote Packages)

To support downloading libraries from a central "Runic Registry", projects use a manifest file.

**`runic.toml`** (Project Manifest):
```toml
[package]
name = "my_sect_artifacts"
version = "1.0.0"

[dependencies]
# Standard registry dependencies
"ancient_scrolls" = "^2.1.0"
"azure_dragon_logic" = "1.2.0"

# Git dependencies
"secret_technique" = { git = "https://github.com/sect/secret.git", tag = "v1.0" }
```

### 4.5 Remote Resolution Process
1.  Compiler checks `runic.toml`.
2.  Downloads missing packages to `~/.runic/cache` or local `libs/`.
3.  Maps imports `ancient_scrolls.*` to the cached location.

## 5. Synthesis Outputs (Calculated Constraints)

The Synthesizer analyzes the circuit to determine the **Minimum Viable Artifact** required to host it. It does *not* take constraints as input; instead, it reports what the circuit demands.

**Reported Metrics:**
*   **Minimum Tier**: Based on node complexity and count (Mortal/Spirit/Earth/Heaven).
*   **Peak Conductivity**: The maximum amplitude detected on any channel (determines material grade needed).
*   **Affinity Bias**: The dominant element(s) in the circuit (suggests optimal material affinity).
*   **Estimated Durability Impact**: Risks from unstable nodes (e.g., `Overrun Emitter`).

**Usage:**
The game engine uses these outputs to prevent engraving a High-Power Fire circuit onto a Low-Grade Wood artifact (which would burn).

## 6. Sub-Elements and Complex Types

**Example:**
```runic
node Transmuter t1 ( source: Wood, target: Fire );
node ThresholdGate gate1 ( threshold: 10 );
```

### 3.4 Connections (Channels)

Connections map outputs to inputs.

```runic
<SourceNode>.<Port> -> <DestNode>.<Port>;
```

*   For Formation ports, use the port name directly.

**Example:**
```runic
ignition -> t1.in;
t1.out -> gate1.in;
gate1.pass -> magma_flow;
```

## 4. Sub-Elements and Complex Types

Since sub-elements like Steam or Magma are dynamic runtime states, the HDL primarily focuses on the *structural* routing of the primary elements. However, ports can be annotated with expected sub-types for validaton.

```runic
output Steam high_pressure_steam; // Semantic alias for Water with metadata
```

## 5. Control Flow Representation

Runic circuits use dataflow control. RunicHDL represents this structurally.

**If/Else (YinYangGate):**
```runic
node YinYangGate check;
input_signal -> check.cond;
data_stream -> check.in;
check.true_out -> processing_path;
check.false_out -> dump_path;
```

## 6. Example: Capacitor-Powered Strike

Based on the spec example.

```runic
formation CapacitorStrike {
    // External interfaces (if this were a sub-module)
    // For a standalone test, we might include sources internally.

    // 1. Source
    node SpiritStoneSocket power_source ( element: Fire, grade: Medium );

    // 2. Storage
    node QiCapacitor cap ( capacity: 50 );

    // 3. Trigger & Release
    // BurstTrigger is defined in Spec Section 4.1 (Capacitor Systems)
    node BurstTrigger trigger;
    
    // 4. Output
    // 'type' parameter tells the Emitter what physical effect to produce
    node EffectEmitter strike ( type: "Fireball" );

    // Wiring
    power_source.out -> cap.in;
    
    // Auto-fire logic: When capacitor is full, trigger the burst
    cap.full -> trigger.trigger;
    cap.out -> trigger.capacitor; // Conceptual wiring, specific ports may vary
    trigger.out -> strike.in;
}
```

## 7. Comments

Standard C-style comments.
*   `// Single line`
*   `/* Multi-line */`

## 8. Compiler/Synthesizer Targets

The RunicHDL compiler should output:
1.  **JSON Graph**: `nodes` and `edges` list importable by the game engine.
2.  **Validation Report**: Checking for type mismatches (e.g., connecting Water to specific Fire inputs without a Combiner).

## 11. Formal Grammar (EBNF)

```ebnf
program         = package_decl, { import_stmt }, { formation } ;
package_decl    = "package", qualified_id, ";" ;
import_stmt     = "import", qualified_id, [ ".*" ], ";" ;

formation       = "formation", identifier, "{", { statement }, "}" ;
statement       = port_def | node_def | connection | comment ;

qualified_id    = identifier, { ".", identifier } ;


port_def        = port_dir, element_type, identifier, [ "[", amplitude_spec, "]" ], ";" ;
port_dir        = "input" | "output" ;
element_type    = "Wood" | "Fire" | "Earth" | "Metal" | "Water" | "Lightning" | "Wind" | "Any" ;
amplitude_spec  = number, "+" | number, "..", number | number ;

node_def        = "node", identifier, identifier, [ "(", param_list, ")" ], ";" ;
param_list      = param, { ",", param } ;
param           = identifier, ":", value ;

connection      = port_ref, "->", port_ref, ";" ;
port_ref        = identifier, [ ".", identifier ] ;

value           = string | number | boolean ;
```

