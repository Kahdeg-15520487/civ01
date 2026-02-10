# Artifact Blueprint Format

> The complete specification for artifact data files combining physical form (OpenSCAD) and circuit logic (RHDL).

## Overview

An artifact blueprint is a JSON package containing:
1. **Physical Definition** - OpenSCAD script for geometry
2. **Component Placement** - Where sockets, emitters, etc. are mounted
3. **Circuit Definition** - RHDL script for runic logic
4. **Port Bindings** - Connection between components and circuit
5. **Calculated Stats** - Material properties, complexity, costs

---

## Blueprint JSON Schema

```json
{
  "$schema": "artifact_blueprint_v1.json",
  "version": "1.0",
  "format": "artifact_blueprint",
  "name": "SkyRazor",
  "description": "A flying sword specialized for aerial combat",
  "author": "PlayerName",
  "created_at": "2024-01-15T10:30:00Z",
  "modified_at": "2024-01-16T14:22:00Z",

  // ===== PHYSICAL DEFINITION (OpenSCAD) =====
  "physical": {
    "script": "artifact_shell.scad",
    "script_hash": "a1b2c3d4e5f6...",

    "material": "ColdIron",
    "material_grade": "Spirit",
    "material_stats": {
      "conductivity": 30,
      "capacity": 40,
      "affinity": "Metal",
      "durability": 70,
      "heat_dissipation": 25,
      "weight": 2.5,
      "poly_cost_multiplier": 1.2
    },

    "polygon_count": 3200,
    "poly_budget": 5000,

    // Component mounts (physical placement)
    "components": [
      {
        "id": "power_socket_1",
        "type": "SpiritStoneSocket",
        "position": [0, 0, 2.5],
        "rotation": [0, 0, 0],
        "model": "std/socket_medium",
        "variant": "socket_3_slot",
        "parameters": {
          "slots": 3,
          "size": "medium"
        }
      },
      {
        "id": "emitter_1",
        "type": "EffectEmitter",
        "position": [60, 0, 0],
        "rotation": [0, 0, 0],
        "emission_type": "SharpnessField",
        "visual": "glow_edge",
        "range": 2.0,
        "parameters": {
          "intensity": 50
        }
      },
      {
        "id": "heat_sink_1",
        "type": "HeatSink",
        "position": [40, 2, 0],
        "rotation": [90, 0, 0],
        "capacity": 50,
        "fin_count": 8,
        "visual": "copper_fins"
      },
      {
        "id": "grounding_rod_1",
        "type": "GroundingRod",
        "position": [0, -15, 0],
        "rotation": [0, 0, 0],
        "length": 20,
        "required_for": ["Lightning", "Plasma"]
      }
    ],

    // Engraving zones (where circuits are placed)
    "engraving_zones": [
      {
        "id": "blade_main",
        "name": "Primary Blade Surface",
        "position": [0, -10, 0],
        "size": [120, 10, 0.1],
        "grid_type": "surface",
        "max_layers": 2
      },
      {
        "id": "hilt_wrap",
        "name": "Hilt Grip Wrap",
        "position": [0, -15, 0],
        "size": [15, 8, 0.1],
        "grid_type": "surface",
        "max_layers": 1
      }
    ]
  },

  // ===== CIRCUIT DEFINITION (RHDL) =====
  "circuit": {
    "script": "formation.rhdl",
    "script_hash": "f6e5d4c3b2a1...",

    "package": "player.artifacts",
    "formation_name": "SkyRazorFlightArray",

    "grid_settings": {
      "grid_type": "surface",
      "layers": 2,
      "trace_width": 1.5,
      "min_spacing": 2.0,
      "via_cost": 5
    },

    // Port bindings: Physical component ↔ Circuit port
    "port_bindings": [
      {
        "component_id": "power_socket_1",
        "component_port": "output",
        "circuit_port": "ignition",
        "trace_type": "power"
      },
      {
        "component_id": "emitter_1",
        "component_port": "input",
        "circuit_port": "sharpness_field",
        "trace_type": "effect"
      },
      {
        "component_id": "heat_sink_1",
        "component_port": "input",
        "circuit_port": "thermal_dump",
        "trace_type": "thermal"
      }
    ],

    // Auto-generated from RHDL compiler
    "nodes": [
      {
        "id": "socket_1",
        "type": "SpiritStoneSource",
        "position": [10, 20, 0]
      },
      {
        "id": "amplifier_1",
        "type": "AmplifierNode",
        "parameters": {
          "gain": 2.0
        },
        "position": [30, 20, 0]
      }
    ],

    "connections": [
      {
        "from_node": "socket_1",
        "from_port": "out",
        "to_node": "amplifier_1",
        "to_port": "in"
      }
    ]
  },

  // ===== CALCULATED STATS =====
  "stats": {
    "conductivity": 30,
    "capacity": 45,
    "affinity": "Metal",
    "durability": 60,
    "weight": 2.5,
    "complexity": 120,
    "heat_generation": 15,
    "heat_dissipation": 40,
    "net_heat": -25,

    "polygon_budget_used": 3200,
    "polygon_budget_max": 5000,
    "component_cost": 380,

    "tier": "Spirit",
    "effective_tier": "Spirit+"
  },

  // ===== VALIDATION RESULTS =====
  "validation": {
    "errors": [],
    "warnings": [
      "Heat generation is near maximum capacity (85%)"
    ],
    "is_valid": true,
    "validated_at": "2024-01-16T14:22:05Z"
  },

  // ===== METADATA =====
  "metadata": {
    "tags": ["weapon", "sword", "flying", "combat"],
    "category": "Weapon",
    "subcategory": "Sword",

    "requirements": {
      "cultivation_level": 100,
      "skill": "Engraving",
      "skill_level": 25
    },

    "sharing": {
      "shareable": true,
      "price": 500,
      "license": "MIT"
    }
  }
}
```

---

## File Structure

A complete artifact blueprint consists of multiple files:

```
artifacts/
└── skyrazor/
    ├── blueprint.json              # Main blueprint file (above)
    ├── artifact_shell.scad         # OpenSCAD geometry script
    ├── formation.rhdl              # Runic circuit script
    ├── artifact.obj                # Compiled mesh (cached)
    ├── preview.png                 # Thumbnail image
    └── components/                 # Custom component models (if any)
        ├── custom_emitter.obj
        └── custom_guard.obj
```

---

## OpenSCAD Script Template

```openscad
// artifact_shell.scad
// Auto-generated or hand-written

include <std/prefabs.scad>

// Artifact metadata (as comments)
// NAME: SkyRazor
// MATERIAL: ColdIron
// GRADE: Spirit

// Main module
module artifact_skyrazor() {
    union() {
        // Blade
        blade_straight(length=120, width=8, thickness=3, tip_ratio=0.3);

        // Hilt
        hilt_curved(length=15, grip="ray_skin")
            .position([0, -15, 0]);

        // Guard
        guard_wing(span=12, angle=30)
            .position([0, 0, 0]);

        // Component mounting points
        component_mount("socket", [0, 0, 2.5]);
        component_mount("emitter", [118, 0, 0]);
        component_mount("heatsink", [40, 0, 2]);

        // Engraving surface indicators
        engraving_zone("blade_main", [0, -10, 0], [120, 10, 3]);
        engraving_zone("hilt_wrap", [0, -15, 0], [15, 8, 3]);
    }
}

// Render
artifact_skyrazor();
```

---

## RHDL Circuit Script

```runic
// formation.rhdl
package player.artifacts;

formation SkyRazorFlightArray {
    input Fire ignition;
    input Wind flow;
    output Kinetic thrust;
    output Heat waste;

    // Power source
    node SpiritStoneSource power(stones: 3);

    // Amplification
    node AmplifierNode amp1(gain: 2.0);
    node AmplifierNode amp2(gain: 1.5);

    // Transmutation
    node TransmuterNode fire_to_wind(from: Fire, to: Wind);

    // Output
    node EffectEmitter thrust_emitter(type: "propulsion");
    node EffectEmitter sharpness_emitter(type: "sharpness");

    // Thermal management
    node HeatSink thermal_dump(capacity: 50);

    // Wiring
    channel ignition -> power.in;
    channel power.out -> amp1.in;
    channel amp1.out -> amp2.in;
    channel amp2.out -> fire_to_wind.in;

    channel fire_to_wind.out -> thrust_emitter.in;
    channel amp1.bypass -> sharpness_emitter.in;

    channel amp2.heat -> thermal_dump.in;
}
```

---

## Component Definition Schema

Each component in the blueprint has this structure:

```json
{
  "id": "unique_component_id",
  "type": "ComponentType",
  "position": [x, y, z],
  "rotation": [x, y, z],
  "model": "path/to/model",
  "parameters": {
    "key": "value"
  },
  "connections": ["circuit_port_1", "circuit_port_2"]
}
```

### Standard Component Types

| Type | Category | Description | Required Parameters |
|------|----------|-------------|---------------------|
| `SpiritStoneSocket` | Power | Accepts Spirit Stone fuel | `slots`, `size` |
| `CultivatorLink` | Power | Draws from cultivator's Qi | `bandwidth`, `range` |
| `EffectEmitter` | Output | Converts Qi to effects | `emission_type`, `intensity`, `range` |
| `HeatSink` | Thermal | Dissipates heat | `capacity`, `fin_count` |
| `GroundingRod` | Safety | Prevents Lightning feedback | `length` |
| `StabilizerNode` | Modifier | Reduces Qi deviation | `strength` |
| `GyroStabilizer` | Flight | Improves flight stability | `torque` |
| `SensorNode` | Input | Detects environmental Qi | `sensitivity`, `element` |

---

## Engraving Zone Definition

```json
{
  "id": "zone_id",
  "name": "Human-readable name",
  "position": [x, y, z],
  "size": [width, height, depth],
  "grid_type": "surface|planar|volumetric",
  "max_layers": 2
}
```

**Grid Types:**
- **surface**: Traces follow the 3D surface of the zone
- **planar**: 2D grid projected onto the surface
- **volumetric**: Full 3D grid (Earth-tier and above)

---

## Port Binding System

The critical link between physical components and circuit logic:

```json
{
  "component_id": "power_socket_1",
  "component_port": "output",
  "circuit_port": "ignition",
  "trace_type": "power"
}
```

**Binding Rules:**
1. Each component port can bind to only one circuit port
2. Multiple component ports can connect to one circuit port (fan-in)
3. One component port can connect to multiple circuit ports (fan-out) - requires splitter
4. Trace type must match component and circuit expectations

**Trace Types:**
- `power` - High-amplitude Qi transmission
- `signal` - Low-amplitude control signals
- `thermal` - Heat dissipation
- `effect` - Physical effect output

---

## Validation Rules

### Physical Validation

```csharp
public class PhysicalValidator
{
    public ValidationResult Validate(ArtifactBlueprint blueprint)
    {
        var result = new ValidationResult();

        // 1. Polygon budget
        if (blueprint.physical.polygon_count > blueprint.physical.poly_budget)
        {
            result.errors.Add($"Polygon count {blueprint.physical.polygon_count} exceeds budget {blueprint.physical.poly_budget}");
        }

        // 2. Material constraints
        var material = MaterialDatabase.Get(blueprint.physical.material);
        if (material.MinimumTier > blueprint.physical.material_grade)
        {
            result.errors.Add($"Material {material.Name} requires minimum tier {material.MinimumTier}");
        }

        // 3. Component limits
        var componentCounts = blueprint.physical.components
            .GroupBy(c => c.type)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var (type, count) in componentCounts)
        {
            var def = ComponentDatabase.Get(type);
            if (def.MaxPerArtifact > 0 && count > def.MaxPerArtifact)
            {
                result.errors.Add($"Too many {type}: {count} > {def.MaxPerArtifact}");
            }
        }

        // 4. Required components
        foreach (var comp in ComponentDatabase.All)
        {
            if (comp.Required && !componentCounts.ContainsKey(comp.Type))
            {
                result.errors.Add($"Missing required component: {comp.Type}");
            }
        }

        return result;
    }
}
```

### Circuit Validation

```csharp
public class CircuitValidator
{
    public ValidationResult Validate(ArtifactBlueprint blueprint)
    {
        var result = new ValidationResult();

        // 1. Port binding completeness
        var circuitPorts = blueprint.circuit.nodes
            .SelectMany(n => n.Ports)
            .ToHashSet();

        var boundPorts = blueprint.circuit.port_bindings
            .Select(b => b.circuit_port)
            .ToHashSet();

        var unboundPorts = circuitPorts.Except(boundPorts);
        if (unboundPorts.Any())
        {
            result.warnings.Add($"Unbound circuit ports: {string.Join(", ", unboundPorts)}");
        }

        // 2. Component port existence
        foreach (var binding in blueprint.circuit.port_bindings)
        {
            var component = blueprint.physical.components
                .FirstOrDefault(c => c.id == binding.component_id);

            if (component == null)
            {
                result.errors.Add($"Binding references non-existent component: {binding.component_id}");
            }
        }

        // 3. Conductivity check
        var maxAmplitude = blueprint.circuit.CalculateMaxAmplitude();
        var materialConductivity = blueprint.physical.material_stats.conductivity;

        if (maxAmplitude > materialConductivity)
        {
            result.errors.Add($"Circuit amplitude {maxAmplitude} exceeds material conductivity {materialConductivity}");
        }

        return result;
    }
}
```

---

## Cache System

Blueprint compilation results are cached for performance:

```json
// cache/index.json
{
  "blueprints": [
    {
      "blueprint_hash": "sha256:abc123...",
      "scad_hash": "sha256:def456...",
      "rhdl_hash": "sha256:789012...",
      "mesh_path": "cache/skyrazor_v3.obj",
      "timestamp": "2024-01-16T14:22:05Z",
      "is_valid": true
    }
  ]
}
```

**Invalidation Triggers:**
- Blueprint JSON modified
- OpenSCAD script modified
- RHDL script modified
- Prefab library updated (version change)
- Material database updated

---

## Sharing & Distribution

### Blueprint Marketplace Format

```json
{
  "marketplace": {
    "id": "bm_skyrazor_001",
    "author": "MasterSmith",
    "version": "2.1",
    "price": 500,

    "ratings": {
      "average": 4.7,
      "count": 234
    },

    "requirements": {
      "tier": "Spirit",
      "cultivation": 100,
      "skills": ["Engraving: 25", "Forging: 15"]
    },

    "files": {
      "blueprint": "blueprint.json",
      "preview": "preview.png",
      "demo_video": "demo.mp4"
    }
  }
}
```

---

## Version History

```json
{
  "version_history": [
    {
      "version": "2.1",
      "date": "2024-01-16",
      "changes": [
        "Added gyro stabilizer",
        "Improved heat distribution",
        "Reduced polygon count by 15%"
      ]
    },
    {
      "version": "2.0",
      "date": "2024-01-10",
      "changes": [
        "Upgraded to Spirit tier",
        "Replaced blade with flame pattern",
        "Added dual emitter configuration"
      ]
    },
    {
      "version": "1.0",
      "date": "2024-01-05",
      "changes": [
        "Initial release"
      ]
    }
  ]
}
```

---

## Open Questions

- [ ] Should blueprints support inheritance/templates?
- [ ] How to handle version compatibility between players?
- [ ] Encryption for paid marketplace blueprints?
- [ ] Blueprint signing for authentication?
