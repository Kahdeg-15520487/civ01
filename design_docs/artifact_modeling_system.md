# Artifact Modeling System Design

> "To shape the vessel is to shape the flow of Qi."

## Overview

The artifact modeling system uses OpenSCAD as the geometric engine, allowing players to programmatically design artifact physical form. This runs parallel to the runic circuit system (RHDL) - physical form and logical circuit are two sides of the same artifact.

---

## Architecture

### Data Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                     Player Input Layer                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │ Visual Drag  │  │ Visual Param │  │ Text Editor  │          │
│  │ & Drop       │  │ Sliders      │  │ (OpenSCAD)   │          │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘          │
└─────────┼──────────────────┼──────────────────┼─────────────────┘
          │                  │                  │
          └──────────────────┼──────────────────┘
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Blueprint Generator                           │
│  ┌────────────────────────┐    ┌────────────────────────┐      │
│  │   OpenSCAD Script      │    │      RHDL Circuit       │      │
│  │   (Geometry + Shell)   │    │      (Logic + Flow)     │      │
│  └────────────┬───────────┘    └────────────┬───────────┘      │
└───────────────┼────────────────────────────┼──────────────────┘
                ▼                            ▼
┌───────────────────────────┐    ┌───────────────────────────┐
│   OpenSCAD Compiler       │    │   RHDL Compiler           │
│   (Bundled Binary)        │    │   (Internal C#)           │
└───────────┬───────────────┘    └───────────┬───────────────┘
            │                                │
            ▼                                ▼
┌───────────────────────────┐    ┌───────────────────────────┐
│   OBJ Mesh (.obj)         │    │   Circuit Graph (JSON)    │
│   (Runtime-loadable)      │    │                           │
└───────────┬───────────────┘    └───────────┬───────────────┘
            │                                │
            └────────────┬───────────────────┘
                         ▼
            ┌──────────────────────────┐
            │   Godot Artifact Node    │
            │   - MeshInstance3D       │
            │   - Circuit Logic        │
            │   - Material Stats       │
            │   - Component Visuals    │
            └──────────────────────────┘
```

---

## Progression Levels

### Level 1: Prefab Stitching (Mortal Tier)

**Target**: Beginners, simple artifacts

**UI**: Drag-and-drop from palette

**Generated Code**:
```openscad
include <std/prefabs.scad>

artifact HeatingStone() {
    union() {
        box(size=[5, 5, 3]);
        translate([0, 0, 3])
        dome(radius=2.5);
        component_mount("socket", [2.5, 2.5, 5]);
        engraving_zone("top", [0, 0, 5.5], [5, 5, 0.1]);
    }
}

HeatingStone();
```

**Features**:
- Basic geometric shapes (box, sphere, cylinder, cone, torus)
- Simple union operations
- Pre-defined component mounts
- Single engraving zone

---

### Level 2: Prefab Customization (Spirit Tier)

**Target**: Intermediate users, enhanced artifacts

**UI**: Parameter sliders + modifier checkboxes

**Generated Code**:
```openscad
use BladeTemplate(straight, length=120, width=8)
    .modify(
        bevel_edges(depth: 2),
        add_fuller(depth: 3),
        hollow_core(pattern: "spiral")
    );
```

**Features**:
- Adjustable prefab parameters (length, width, etc.)
- Shape modifiers (bevel, fuller, hollow, carve_pattern)
- Multiple component mounts
- Multiple engraving zones
- Basic positioning/rotation

---

### Level 3: Custom Prefab Design (Earth Tier)

**Target**: Advanced users, custom components

**UI**: Prefab editor with syntax highlighting

**Example - Custom Blade**:
```openscad
// Player creates: blade_flame.scad
module blade_flame(length=100, width=8) {
    difference() {
        // Base blade
        hull() {
            cube([2, width, 1], center=true);
            translate([0, length, 0])
            scale([0.2, 0.2, 1])
            cube([2, width, 1], center=true);
        }
        // Flame edge pattern
        for (i = [0:length:5]) {
            translate([0, i, -0.5])
            scale([1, 1, 2])
            sphere(r=0.5);
        }
    }
}
```

**Then Use in Main Artifact**:
```openscad
use player.custom.blade_flame;
blade_flame(length=120, width=10);
```

**Features**:
- Custom CSG operations (union, difference, intersection, hull)
- Loops and conditionals
- Custom modules
- Prefab library contributions

---

### Level 4: From Scratch (Heaven Tier)

**Target**: Masters, complete creative freedom

**UI**: Full OpenSCAD code editor with autocomplete

**Example**:
```openscad
module CustomArtifact() {
    // Complete CSG freedom
    difference() {
        union() {
            // Complex custom geometry
            for (i = [0:7]) {
                rotate([0, 0, i * 45])
                translate([10, 0, 0])
                cylinder(h=20, r=2);
            }
            sphere(r=8);
        }
        // Cutouts
        for (i = [0:3]) {
            rotate([0, 90, i * 90])
            cylinder(h=30, r=1, center=true);
        }
    }
}
```

**Features**:
- All OpenSCAD language features
- Parametric functions
- Complex transformations
- No prefab restrictions

---

### Level 5: External Import (Post-Game)

**Target**: Community sharing, external tools

**UI**: File upload with validation

**Validation Requirements**:
```csharp
public struct ArtifactConstraints
{
    public int maxPolygons;        // Based on tier
    public string[] allowedMaterials;
    public float maxSize;          // in mm
    public int maxComponents;
    public bool requiresComponentMounts;
    public bool requiresEngravingZones;
}
```

**Required Modules in External .scad**:
- `component_mount(type, position)` - At least one
- `engraving_zone(name, position, size)` - At least one
- No forbidden operations (import, surface, projection for external files)

---

## OpenSCAD Integration

### Binary Distribution

**Path Structure**:
```
GameDistribution/
├── Game.exe
├── game_data/
├── tools/
│   ├── openscad/
│   │   ├── openscad.exe        (GPL v2, unmodified)
│   │   ├── COPYING             (GPL license)
│   │   └── README.txt          (Usage notes)
│   └── config/
└── licenses/
    ├── openscad_license.txt
    └── third_party_licenses.txt
```

**Licensing Note**: OpenSCAD is executed as a separate process (CLI), not linked into the game executable. This keeps the game proprietary while complying with GPL v2. The game communicates with OpenSCAD through files and command-line arguments only.

---

### C# Bridge Implementation

```csharp
// OpenSCADBridge.cs
public class OpenSCADBridge
{
    private string _openscadPath;

    public struct CompileResult
    {
        public bool success;
        public string meshPath;
        public string errorLog;
        public int polygonCount;
    }

    public OpenSCADBridge()
    {
        _openscadPath = Path.Combine(
            OS.GetExecutableDir().GetBaseDir(),
            "tools",
            "openscad",
            "openscad.exe"
        );
    }

    public CompileResult Compile(string scadScript, string outputDir)
    {
        var result = new CompileResult();

        try
        {
            string scriptPath = Path.Combine(outputDir, "temp.scad");
            string objPath = Path.Combine(outputDir, "artifact.obj");

            File.WriteAllText(scriptPath, scadScript);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _openscadPath,
                    Arguments = $"-o \"{objPath}\" \"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0 && File.Exists(objPath))
            {
                result.success = true;
                result.meshPath = objPath;
                result.polygonCount = CountPolygons(objPath);
            }
            else
            {
                result.success = false;
                result.errorLog = error;
            }
        }
        catch (Exception e)
        {
            result.success = false;
            result.errorLog = e.Message;
        }

        return result;
    }
}
```

---

## Runtime Mesh Loading

### Format Selection: OBJ

**Why OBJ?**
- ✓ OpenSCAD natively exports OBJ
- ✓ Simple text format (easy to debug)
- ✓ Runtime loadable in Godot
- ✓ No external dependencies

### OBJ Parser Implementation

```csharp
// OBJParser.cs
public class OBJParser
{
    public ArrayMesh Parse(string objPath)
    {
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();

        foreach (var line in File.ReadLines(objPath))
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            switch (parts[0])
            {
                case "v":  // Vertex
                    vertices.Add(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    ));
                    break;

                case "vn":  // Normal
                    normals.Add(new Vector3(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture),
                        float.Parse(parts[3], CultureInfo.InvariantCulture)
                    ));
                    break;

                case "vt":  // UV coordinate
                    uvs.Add(new Vector2(
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture)
                    ));
                    break;

                case "f":  // Face
                    // OBJ format: f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
                    foreach (var part in parts.Skip(1))
                    {
                        var indices_str = part.Split('/');
                        int vIdx = int.Parse(indices_str[0]) - 1;  // OBJ is 1-indexed
                        indices.Add(vIdx);
                    }
                    break;
            }
        }

        return CreateMesh(vertices, normals, uvs, indices);
    }

    private ArrayMesh CreateMesh(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> indices)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        var vertArray = new Godot.Collections.Vector3Array();
        foreach (var v in vertices) vertArray.Add(v);
        arrays[(int)Mesh.ArrayType.Vertex] = vertArray;

        if (normals.Count > 0)
        {
            var normalArray = new Godot.Collections.Vector3Array();
            foreach (var n in normals) normalArray.Add(n);
            arrays[(int)Mesh.ArrayType.Normal] = normalArray;
        }

        if (uvs.Count > 0)
        {
            var uvArray = new Godot.Collections.Vector2Array();
            foreach (var uv in uvs) uvArray.Add(uv);
            arrays[(int)Mesh.ArrayType.TexUV] = uvArray;
        }

        var indexArray = new Godot.Collections.Int32Array();
        foreach (var i in indices) indexArray.Add(i);
        arrays[(int)Mesh.ArrayType.Index] = indexArray;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return mesh;
    }
}
```

---

## Prefab Library

### Standard Library Structure

```
std/
├── prefabs.scad              # Main prefab definitions
├── basic/
│   ├── box(size)
│   ├── sphere(radius)
│   ├── cylinder(h, r)
│   ├── cone(h, r1, r2)
│   └── torus(r1, r2)
├── weapon/
│   ├── blade_straight(length, width, thickness, tip_ratio, edge_type)
│   ├── blade_curved(length, curve_depth, width)
│   ├── axe_head(blade_width, poll_length)
│   ├── hammer_head(weight, face_size)
│   └── arrowhead(length, width, style)
├── tool/
│   ├── handle(length, grip_type)
│   ├── hilt(length, guard_type, pommel_type)
│   ├── guard(style, span, angle)
│   └── pommel(weight, style)
└── component/
    ├── socket_mount(size, type)
    ├── emitter_mount(type)
    ├── heat_sink(size, fin_count)
    └── grounding_rod(length)
```

### Prefab Module Template

```openscad
// blade_straight.scad - Version 2.0 (Current)
// PREFAB_VERSION: 2
// TIER: Mortal
// PARAMETERS: 5

module blade_straight(
    length = 100,
    width = 6,
    thickness = 2,
    tip_ratio = 0.3,
    edge_type = "flat"  // flat, beveled, serrated
) {
    // Base blade shape
    base = hull() {
        cube([thickness, width, 1], center=true);
        translate([0, length, 0])
        scale([tip_ratio, tip_ratio, 1])
        cube([thickness, width, 1], center=true);
    };

    // Apply edge treatment
    if (edge_type == "beveled") {
        bevel_edges(base, depth=1);
    } else if (edge_type == "serrated") {
        add_serrations(base, spacing=5, depth=1);
    } else {
        base;  // Flat edge (default)
    }
}
```

### Modifier Operations

```openscad
// Standard modifiers available to players

// Bevel all edges
module bevel_edges(depth=0.5) {
    difference() {
        children();
        // Bevel logic...
    }
}

// Add blood groove (fuller)
module add_fuller(depth=2, width=1.5, position="center") {
    difference() {
        children();
        translate([0, 0, -depth/2])
        cube([width, 200, depth], center=true);
    }
}

// Hollow out core
module hollow_core(pattern="straight", depth=3) {
    difference() {
        children();
        if (pattern == "straight") {
            translate([0, 0, -depth/2])
            cube([depth, 200, depth], center=true);
        } else if (pattern == "spiral") {
            linear_extrude(height=120, twist=720, center=false)
            translate([depth/2, 0, 0])
            circle(r=depth/2);
        }
    }
}

// Carve decorative pattern
module carve_pattern(pattern, depth=1) {
    difference() {
        children();
        if (pattern == "waves") {
            for (i = [0:10:100]) {
                translate([0, i, -depth])
                scale([1, 5, 1])
                sphere(r=1);
            }
        } else if (pattern == "dragon") {
            // Dragon scale pattern...
        }
    }
}

// Extend or shorten
module extend_tip(amount=5) {
    // Add length to end
}

// Add gem sockets
module add_sockets(count, spacing="equal") {
    // Place socket mounting points
}
```

---

## Component Integration

### Component Mounts

Components (sockets, emitters, heat sinks) are physically mounted on the artifact:

```openscad
// Define where components attach
component_mount("socket", [60, 0, 1.5]);
component_mount("emitter", [118, 0, 0]);
component_mount("heatsink", [40, 0, 2]);
```

### Visual Markers

```openscad
module component_mount(type, position) {
    color([0, 1, 0, 0.5])
    translate(position)
    if (type == "socket") {
        cylinder(r=2, h=1);
    } else if (type == "emitter") {
        sphere(r=1.5);
    } else if (type == "heatsink") {
        cube([3, 3, 2], center=true);
    }
}
```

---

## Engraving Zones

The physical surface where runic circuits are engraved:

```openscad
// Define valid circuit areas
engraving_zone("blade_main", [0, -10, 0], [120, 10, 3]);
engraving_zone("hilt_wrap", [0, -15, 0], [15, 8, 3]);
```

**Visual Indicator**:
```openscad
module engraving_zone(name, position, size) {
    color([1, 1, 0, 0.3])
    translate(position)
    cube(size, center=false);
}
```

**Grid Generation Rules**:
| Zone Shape | Grid Type | Notes |
|------------|-----------|-------|
| Flat surface | Planar 2D grid | Standard |
| Curved surface | Unwrapped 2D | UV projection |
| Complex 3D | Volumetric grid | Earth-tier+ |
| Hollow interior | Internal grid | Requires vias |

---

## Error Handling

### Error Parser

```csharp
public class OpenSCADErrorParser
{
    private Dictionary<string, string> _errorMappings = new()
    {
        { "undefined variable", "You're using a variable that doesn't exist. Check for typos." },
        { "syntax error", "There's a typo in your code. Check the line marked in red." },
        { "parameter undefined", "This prefab doesn't have that parameter. Check the prefab documentation." },
        { "module not found", "This prefab doesn't exist. Check the prefab library." },
        { "CSG error", "The shapes can't be combined this way. Try adjusting the sizes or positions." }
    };

    public List<GameMessage> Parse(string rawOutput)
    {
        var messages = new List<GameMessage>();

        foreach (var line in rawOutput.Split('\n'))
        {
            if (line.Contains("ERROR"))
            {
                var msg = new GameMessage
                {
                    type = MessageType.Error,
                    text = ExtractErrorText(line),
                    line = ExtractLineNumber(line),
                    friendlyText = MapToFriendlyMessage(line)
                };
                messages.Add(msg);
            }
        }

        return messages;
    }
}
```

**Since players only use prefab functions, cryptic OpenSCAD errors are minimized.**

---

## Open Questions

- [ ] Should LODs be generated in OpenSCAD (multiple $fn passes) or in Godot (mesh simplification)?
- [ ] How to handle prefabs that become obsolete between game versions?
- [ ] Maximum polygon budget per tier?
- [ ] Multiplayer blueprint sharing validation?
