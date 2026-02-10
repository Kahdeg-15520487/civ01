# OpenSCAD Integration Test

This directory contains the OpenSCAD bridge and OBJ parser for converting OpenSCAD scripts to Godot meshes.

## Files

### Backend
- `OpenSCADBridge.cs` - Manages OpenSCAD CLI execution
- `OBJParser.cs` - Parses OBJ files to Godot ArrayMesh
- `OpenSCADTestController.cs` - Test scene controller
- `OpenSCADTestScene.tscn` - Test scene

## Setup

### 1. Install OpenSCAD

Download and install OpenSCAD for your platform:
- Windows: https://openscad.org/downloads.html
- Linux: `sudo apt-get install openscad`
- macOS: `brew install openscad`

### 2. Bundle OpenSCAD with Game

Copy the OpenSCAD binary to the game's tools directory:

```
game/
└── tools/
    └── openscad/
        ├── openscad.exe (Windows)
        ├── openscad (Linux/macOS)
        └── (other OpenSCAD files)
```

**Windows**: Copy `openscad.exe` and all DLLs from the installation directory.

### 3. Run Test Scene

1. Open the project in Godot
2. Open `scenes/artifacts/OpenSCADTestScene.tscn`
3. Press F6 to run the scene

## Test Process

1. Enter OpenSCAD script in the text box
2. Click "Compile & Render"
3. Pipeline:
   ```
   Script → OpenSCAD CLI → OBJ file → Godot Mesh → Display
   ```
4. Use WASD to move camera, right-click + drag to rotate

## Example Scripts

### Simple Cylinder
```
cylinder(h=10, r=5, center=false);
```

### Box
```
cube([5, 5, 5], center=true);
```

### Sphere
```
sphere(r=5);
```

### Combination
```
union() {
    cube([5, 5, 5], center=true);
    translate([0, 3, 0])
    sphere(r=2);
}
```

### Using difference
```
difference() {
    cube([5, 5, 5], center=true);
    sphere(r=3);
}
```

## Troubleshooting

### "OpenSCAD binary not found"
- Ensure OpenSCAD is in `tools/openscad/` directory
- Check the path in `OpenSCADBridge.cs` InitializePaths()

### "Compilation failed"
- Check the OpenSCAD script for syntax errors
- OpenSCAD error messages will be shown in the status label
- Try running the script in OpenSCAD directly to debug

### Nothing renders
- Check that OBJ file was created successfully
- Verify polygon count is within budget (1000 for Mortal tier)
- Look for GD.Print messages in the Godot output console

## Architecture

```
┌─────────────────┐
│ Godot Scene     │
│ (Input Script)  │
└────────┬────────┘
         │
         ▼
┌─────────────────────────┐
│ OpenSCADBridge.cs       │
│ - Creates temp .scad     │
│ - Executes OpenSCAD CLI  │
│ - Waits for .obj output │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│ OpenSCAD CLI            │
│ (External Process)       │
│ - Reads .scad            │
│ - Generates .obj         │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│ OBJParser.cs            │
│ - Parses .obj file      │
│ - Creates ArrayMesh      │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│ MeshInstance3D          │
│ (Displays in 3D)         │
└─────────────────────────┘
```

## License Note

OpenSCAD is licensed under GPL v2. This integration executes OpenSCAD as a separate process (CLI), which keeps your game proprietary while complying with GPL terms.
