# Artifact Forge Scene Design

> The complete artifact modeling and creation interface

## Overview

The **ArtifactForgeScene** is a dedicated scene where players transform their saved runic formations into physical artifacts. This scene handles:
- Formation selection
- Material selection
- 3D artifact modeling (via OpenSCAD)
- Component placement
- Blueprint validation
- Artifact forging

---

## Scene Structure

```
ArtifactForgeScene.tscn
├── Root (Control)
│   ├── Background (ColorRect) - Sect interior backdrop
│   ├── Header (HBoxContainer)
│   │   ├── Title Label
│   │   ├── Formation Counter
│   │   └── Return Button
│   ├── Main Content (HSplitContainer)
│   │   ├── Left Panel (VBoxContainer)
│   │   │   ├── Formation Library
│   │   │   └── Material Selector
│   │   └── Right Panel (VBoxContainer)
│   │       ├── 3D Viewport
│   │       ├── Prefab Palette
│   │       ├── Code View
│   │       └── Properties Panel
│   └── Bottom Panel (Panel)
│       ├── Validation Info
│       └── Action Buttons
```

---

## Complete UI Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│  Artifact Forge - Azure Peak Sect                      [Return]      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌───────────────────┐  ┌─────────────────────────────────────────┐ │
│  │ Formation Library │  │                                          │ │
│  │ ┌───────────────┐ │  │          3D Viewport (OpenCSG Preview)   │ │
│  │ │ Simple Heater │ │  │                                          │ │
│  │ │ Water Purify  │ │  │           [Artifact Mesh]                │ │
│  │ │ Sharp Blade   │ │  │                                          │ │
│  │ │               │ │  │    🔵 Power Socket (component)          │ │
│  │ │ [Load New]    │ │  │                                          │ │
│  │ └───────────────┘ │  │    🟨 Engraving Zone                   │ │
│  └───────────────────┘  │                                          │ │
│  ┌───────────────────┐  │         ⬇ Rotate                        │ │
│  │ Material          │  │        ⬅ Pan ➡                         │ │
│  │ ┌───────────────┐ │  │       🔍+ Zoom 🔍-                       │ │
│  │ │ Spirit Stone  │ │  │                                          │ │
│  │ │ Cost: 100     │ │  │   Grid: 20px | Snap: ON                  │ │
│  │ │ Cond: 15      │ │  │                                          │ │
│  │ │               │ │  └─────────────────────────────────────────┘ │
│  │ │ [Locked]      │ │  ┌─────────────────────────────────────────┐ │
│  │ │ Cold Iron     │  │          Prefab Palette                   │ │
│  │ │ Cost: 500     │  │  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐        │ │
│  │ │ Requires: Lv5 │  │  │ Box │ │Sphere│ │Cyl  │ │Cone │        │ │
│  │ └───────────────┘ │  │  └─────┘ └─────┘ └─────┘ └─────┘        │ │
│  └───────────────────┘  │                                          │ │
│                         │  Weapon: [Blade] [Hilt] [Guard]         │ │
│  Selected: Simple Heater│  Tool: [Handle] [Socket] [Emitter]      │ │
│  Material: Spirit Stone │  Component: [Mount] [Sink] [Ground]      │ │
│  Complexity: 25/100     │  └──────────────────────────────────────────┘ │
│                         │  ┌─────────────────────────────────────────┐ │
│  ┌───────────────────┐  │  │         OpenSCAD Code                   │ │
│  │ Design Progress    │  │  ┌─────────────────────────────────────┐ │ │
│  │ ┌─────────────┐   │  │  │ artifact heating_stone() {          │ │ │
│  │ │□□□□□□□□□□□│ 50%│ │  │   // Auto-generated from prefab       │ │ │
│  │ └─────────────┘   │  │  │   use Box(size=[5,5,3]);             │ │ │
│  │                   │  │  │   use Dome(r=2.5)                  │ │ │
│  │ Required:          │  │  │                                     │ │ │
│  │ ☐ Power Socket    │  │  │   component_mount("socket",          │ │ │
│  │ ☐ Effect Emitter  │  │  │     [2.5,2.5,3]);                  │ │ │
│  │ ☐ Engraving Zone  │  │  │                                     │ │ │
│  └───────────────────┘  │  │   engraving_zone("top",              │ │ │
│                         │  │     [0,0,3], [5,5,0.1]);            │ │ │
│                         │  │ }                                    │ │ │
│                         │  │ heating_stone();                     │ │ │
│                         │  └─────────────────────────────────────┘ │ │
│                         │  [Edit Code] [Compile] [Save Prefab]    │ │
│                         │  └──────────────────────────────────────────┘ │
│                         │  ┌─────────────────────────────────────────┐ │
│                         │  │         Properties                       │ │ │
│                         │  │  Selected: Box                         │ │ │
│                         │  │  ┌─────────────────────────────────┐   │ │
│                         │  │  │ Position: [0▾][0▾][0▾]       │   │ │
│                         │  │  │ Size: [5▾] x [5▾] x [3▾]     │   │ │
│                         │  │  │ Material: Spirit Stone         │   │ │
│                         │  │  │                                 │   │ │
│                         │  │  │ [Add to Design] [Remove]       │   │ │
│                         │  │  └─────────────────────────────────┘   │ │
│                         │  └──────────────────────────────────────────┘ │
│                         │                                                 │
│                         └─────────────────────────────────────────────┘ │
│                                                                       │
│  ┌───────────────────────────────────────────────────────────────┐   │
│  │ Status: Ready to Compile                                        │   │
│  │ Components: 1/3 placed | Polygons: ? | Material OK ✓          │   │
│  │                                                                  │   │
│  │  [Generate OpenSCAD]  [Compile Mesh]  [Test Circuit]  [Forge Artifact] │
│  └───────────────────────────────────────────────────────────────┘   │
└───────────────────────────────────────────────────────────────────────┘
```

---

## Workflow States

### State 1: Formation Selection (Entry)

```
Player enters scene
        ↓
Show formation library (saved formations)
        ↓
Player selects formation
        ↓
Show circuit analysis:
- Primary output type
- Max amplitude
- Complexity
- Suggested artifact types
        ↓
Enable material selection
```

### State 2: Material Selection

```
Show available materials (filtered by player tier)
        ↓
For each material, show:
- Name
- Cost
- Conductivity
- Affinity
- Special properties
        ↓
Player selects material
        ↓
Validate against circuit:
✓ If max amplitude ≤ conductivity
✗ Else: Show warning, require stabilizer or reduce power
        ↓
Enable artifact modeling
```

### State 3: Artifact Type Selection

```
Based on circuit, suggest artifact types:
┌─────────────────────────────────────┐
│ Suggested Artifacts                │
├─────────────────────────────────────┤
│ 🔥 Heating Stone (Utility)         │
│    Output: 50 Qi/tick              │
│    Size: Small                     │
│    [Select]                        │
│                                    │
│ 🏺 Tea Kettle (Utility)            │
│    Output: 40 Qi/tick              │
│    Size: Medium                    │
│    [Select]                        │
│                                    │
│ ⚒ Furnace (Production)            │
│    Output: 200 Qi/tick             │
│    Size: Large                    │
│    [Locked - Requires Spirit Tier] │
└─────────────────────────────────────┘

Player selects type OR chooses "Custom"
        ↓
Load appropriate prefab palette
        ↓
Enable 3D viewport
```

### State 4: Artifact Modeling

```
3D Viewport activates
        ↓
Player drags prefabs from palette
        ↓
Prefabs snap to grid in viewport
        ↓
Auto-generate OpenSCAD code in real-time
        ↓
Update component requirements checklist
        ↓
Player adjusts parameters in properties panel
        ↓
Design complete when all requirements met:
✓ At least 1 power source
✓ At least 1 effect emitter
✓ At least 1 engraving zone
✓ Components aligned with circuit ports
```

### State 5: Compilation & Validation

```
Player clicks "Generate OpenSCAD"
        ↓
Scene generates final OpenSCAD script
        ↓
Player clicks "Compile Mesh"
        ↓
OpenSCADBridge.Execute(script)
        ↓
Parse OBJ to Godot mesh
        ↓
Show in 3D viewport
        ↓
Validate:
✓ Polygon count within budget
✓ All components present
✓ Material conductivity OK
✓ Heat generation acceptable
        ↓
If any validation fails:
- Show error message
- Highlight problematic area
- Allow corrections
        ↓
If all validation passes:
- Enable "Forge Artifact" button
```

### State 6: Artifact Forging

```
Player clicks "Forge Artifact"
        ↓
Show final confirmation dialog:
┌─────────────────────────────────────────┐
│ Forge this artifact?                    │
│                                         │
│ Name: [My Heating Stone___________]     │
│                                         │
│ Preview:                                │
│    [3D thumbnail]                        │
│                                         │
│ Stats:                                  │
│ • Output: 50 Heat Qi/tick               │
│ • Duration: 60 min/spirit stone         │
│ • Complexity: 25                        │
│ • Durability: 50                        │
│                                         │
│ Cost:                                   │
│ • Material: Spirit Stone (100)          │
│ • Components: (50)                      │
│ • Crafting: (Free)                      │
│ ─────────────────────────────────       │
│ Total: 150 Spirit Coins                 │
│                                         │
│ [Cancel]              [Forge Artifact]  │
└─────────────────────────────────────────┘
        ↓
If confirmed:
- Deduct cost
- Create ArtifactInstance
- Save blueprint
- Add to inventory
- Award XP
- Update quest progress
        ↓
Show success dialog
        ↓
Return to ArtifactForge (ready for next artifact)
```

---

## Component: Formation Library

```gdscript
# FormationLibraryPanel.gd
extends VBoxContainer

signal formation_selected(formation_data)

var _formation_list: ItemList
var _formations: Array = []

func _ready():
    _setup_ui()
    _load_formations()

func _setup_ui():
    add_child(Label.new("Formation Library"))

    _formation_list = ItemList.new()
    _formation_list.custom_minimum_size = Vector2(200, 150)
    _formation_list.item_selected.connect(_on_formation_selected)
    add_child(_formation_list)

    var load_btn = Button.new()
    load_btn.text = "Load New Formation"
    load_btn.pressed.connect(_on_load_new)
    add_child(load_btn)

func _load_formations():
    _formations = FormationManager.get_all_formations()
    _formation_list.clear()

    for formation in _formations:
        var stats = formation.stats
        var text = "%s\n  Nodes: %d | Complex: %d" % [
            formation.name,
            stats.node_count,
            stats.complexity
        ]
        _formation_list.add_item(text)

func _on_formation_selected(index: int):
    var formation = _formations[index]
    formation_selected.emit(formation)

func _on_load_new():
    # Open dialog to import formation from file
    pass
```

---

## Component: Material Selector

```gdscript
# MaterialSelectorPanel.gd
extends VBoxContainer

signal material_selected(material_data)

var _material_buttons: Array = []
var _selected_material: String = ""

func _ready():
    _setup_ui()

func _setup_ui():
    add_child(Label.new("Material"))

    # Load from MaterialDatabase
    var materials = MaterialDatabase.get_unlocked_materials()

    for mat in materials:
        var btn = _create_material_button(mat)
        _material_buttons.append(btn)
        add_child(btn)

func _create_material_button(material: Dictionary) -> Control:
    var container = VBoxContainer.new()

    # Check if locked
    var is_locked = material.get("locked", false)

    var btn = Button.new()
    btn.text = material.name
    btn.disabled = is_locked
    btn.pressed.connect(_on_material_pressed.bind(material.id))

    container.add_child(btn)

    if not is_locked:
        var stats = Label.new()
        stats.text = "Cost: %d | Cond: %d" % [
            material.cost,
            material.conductivity
        ]
        stats.add_theme_font_size_override("font_size", 12)
        container.add_child(stats)
    else:
        var lock_info = Label.new()
        lock_info.text = "Requires: " + material.requirement
        lock_info.add_theme_font_size_override("font_size", 12)
        lock_info.add_theme_color_override("font_color", Color.RED)
        container.add_child(lock_info)

    return container

func _on_material_pressed(material_id: String):
    _selected_material = material_id
    material_selected.emit(MaterialDatabase.get(material_id))
```

---

## Component: 3D Viewport

```gdscript
# ArtifactViewport.gd
extends Control

var _camera: Camera3D
var _mesh_instance: MeshInstance3D
var _grid_helper: GridHelper
var _preview_mode: bool = true

# OpenSCAD integration
var _openscad_bridge: OpenSCADBridge
var _current_mesh: ArrayMesh

signal mesh_generated(mesh: ArrayMesh)
signal component_placed(component_data)

func _ready():
    _setup_3d_scene()
    _setup_input()

func _setup_3d_scene():
    # Create SubViewport for 3D rendering
    var viewport = SubViewport.new()
    viewport.size = size
    viewport.transparent_bg = true
    add_child(viewport)

    # Create viewport container to display
    var vp_container = SubViewportContainer.new()
    vp_container.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
    vp_container.stretch = true
    add_child(vp_container)
    vp_container.add_child(viewport)

    # Setup scene
    var root = Node3D.new()
    viewport.add_child(root)

    # Camera
    _camera = Camera3D.new()
    _camera.position = Vector3(5, 5, 5)
    _camera.look_at(Vector3.ZERO)
    root.add_child(_camera)

    # Lighting
    var light = DirectionalLight3D.new()
    light.position = Vector3(10, 10, 10)
    root.add_child(light)

    var ambient = AmbientLight3D.new()
    ambient.ambient_color = Color(0.3, 0.3, 0.3)
    root.add_child(ambient)

    # Grid helper
    _grid_helper = GridHelper.new()
    root.add_child(_grid_helper)

    # Mesh instance
    _mesh_instance = MeshInstance3D.new()
    root.add_child(_mesh_instance)

func _setup_input():
    # Mouse controls for camera
    pass

func load_mesh(mesh_path: String):
    # Load compiled OBJ mesh
    var mesh = load(mesh_path)
    if mesh is ArrayMesh:
        _current_mesh = mesh
        _mesh_instance.mesh = mesh
        mesh_generated.emit(mesh)

func clear_mesh():
    _mesh_instance.mesh = null
    _current_mesh = null

func add_component_marker(position: Vector3, type: String):
    # Add visual marker for component placement
    var marker = MeshInstance3D.new()

    match type:
        "socket":
            marker.mesh = _create_socket_marker()
        "emitter":
            marker.mesh = _create_emitter_marker()
        "engraving":
            marker.mesh = _create_engraving_marker()

    marker.position = position
    _mesh_instance.add_child(marker)

func _create_socket_marker() -> PrimitiveMesh:
    var sphere = SphereMesh.new()
    sphere.radius = 0.2
    sphere.radial_segments = 16
    return sphere

func _create_emitter_marker() -> PrimitiveMesh:
    var box = BoxMesh.new()
    box.size = Vector3(0.3, 0.3, 0.3)
    return box

func _create_engraving_marker() -> PrimitiveMesh:
    var plane = PlaneMesh.new()
    plane.size = Vector2(1, 1)
    return plane
```

---

## Component: Prefab Palette

```gdscript
# PrefabPalette.gd
extends VBoxContainer

signal prefab_dragged(prefab_data)
signal prefab_selected(prefab_data)

enum Category {
    BASIC,
    WEAPON,
    TOOL,
    COMPONENT
}

var _current_category: Category = Category.BASIC
var _prefab_buttons: Array = []

func _ready():
    _setup_category_tabs()
    _setup_prefab_grid()

func _setup_category_tabs():
    var tabs = TabBar.new()
    tabs.add_tab("Basic")
    tabs.add_tab("Weapon")
    tabs.add_tab("Tool")
    tabs.add_tab("Component")
    tabs.tab_changed.connect(_on_category_changed)
    add_child(tabs)

func _setup_prefab_grid():
    var grid = GridContainer.new()
    grid.columns = 4
    add_child(grid)

    _load_prefabs_for_category(_current_category, grid)

func _load_prefabs_for_category(category: Category, grid: GridContainer):
    # Clear existing
    for child in grid.get_children():
        child.queue_free()

    var prefabs = PrefabLibrary.get_by_category(category)

    for prefab in prefabs:
        var btn = _create_prefab_button(prefab)
        grid.add_child(btn)

func _create_prefab_button(prefab: Dictionary) -> Control:
    var container = VBoxContainer.new()

    var btn = Button.new()
    btn.text = prefab.display_name
    btn.custom_minimum_size = Vector2(60, 40)
    btn.pressed.connect(_on_prefab_clicked.bind(prefab))
    container.add_child(btn)

    # Drag support
    btn.drag_started.connect(_on_prefab_drag_started.bind(prefab))

    return container

func _on_prefab_clicked(prefab: Dictionary):
    prefab_selected.emit(prefab)

func _on_prefab_drag_started(prefab: Dictionary):
    prefab_dragged.emit(prefab)

func _on_category_changed(index: int):
    _current_category = index as Category
    _setup_prefab_grid()
```

---

## Component: OpenSCAD Code View

```gdscript
# OpenSCADCodeView.gd
extends CodeEdit

var _current_script: String = ""
var _auto_generated: bool = true

func _ready():
    syntax_highlighting = true
    wrap_mode = true
    readonly = true  # Read-only for auto-generated

    # Add toolbar
    var toolbar = HBoxContainer.new()
    add_child(toolbar)

    var edit_btn = Button.new()
    edit_btn.text = "Edit Code"
    edit_btn.pressed.connect(_on_edit_clicked)
    toolbar.add_child(edit_btn)

    var save_btn = Button.new()
    save_btn.text = "Save as Prefab"
    save_btn.pressed.connect(_on_save_prefab_clicked)
    toolbar.add_child(save_btn)

func set_script(script: String):
    _current_script = script
    text = script

func _on_edit_clicked():
    # Toggle edit mode
    readonly = not readonly
    if not readonly:
        _auto_generated = false  # Now manual

func _on_save_prefab_clicked():
    # Save current script as custom prefab
    PrefabLibrary.save_custom_prefab(text)
```

---

## Component: Validation Panel

```gdscript
# ValidationPanel.gd
extends Panel

var _requirement_labels: Dictionary = {}
var _status_labels: Dictionary = {}

func _ready():
    _setup_ui()

func _setup_ui():
    var vbox = VBoxContainer.new()
    add_child(vbox)

    # Requirements section
    var req_label = Label.new("Required Components:")
    req_label.add_theme_font_size_override("font_size", 14)
    vbox.add_child(req_label)

    _add_requirement_check("Power Source", "power_source", vbox)
    _add_requirement_check("Effect Emitter", "effect_emitter", vbox)
    _add_requirement_check("Engraving Zone", "engraving_zone", vbox)

    # Status section
    var status_label = Label.new("Status:")
    status_label.add_theme_font_size_override("font_size", 14)
    vbox.add_child(status_label)

    _add_status_label("Components", "components", vbox)
    _add_status_label("Polygons", "polygons", vbox)
    _add_status_label("Material", "material", vbox)
    _add_status_label("Heat", "heat", vbox)

func _add_requirement_check(name: String, key: String, parent: VBoxContainer):
    var hbox = HBoxContainer.new()
    parent.add_child(hbox)

    var checkbox = CheckBox.new()
    checkbox.disabled = true
    _requirement_labels[key] = checkbox
    hbox.add_child(checkbox)

    var label = Label.new(name)
    hbox.add_child(label)

func _add_status_label(name: String, key: String, parent: VBoxContainer):
    var hbox = HBoxContainer.new()
    parent.add_child(hbox)

    var label = Label.new(name + ": ")
    hbox.add_child(label)

    var value = Label.new("---")
    _status_labels[key] = value
    hbox.add_child(value)

func update_requirement(key: String, met: bool):
    if key in _requirement_labels:
        _requirement_labels[key].button_pressed = met

func update_status(key: String, text: String, valid: bool):
    if key in _status_labels:
        _status_labels[key].text = text
        _status_labels[key].add_theme_color_override(
            "font_color",
            Color.GREEN if valid else Color.RED
        )
```

---

## Main Controller: ArtifactForgeController

```csharp
// ArtifactForgeController.cs
using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Civ.Artifacts;

/// <summary>
/// Main controller for the Artifact Forge scene.
/// Orchestrates the entire artifact creation workflow.
/// </summary>
[GlobalClass]
public partial class ArtifactForgeController : Node
{
    // UI References
    private FormationLibraryPanel _formationLibrary;
    private MaterialSelectorPanel _materialSelector;
    private ArtifactViewport _viewport;
    private PrefabPalette _palette;
    private OpenSCADCodeView _codeView;
    private ValidationPanel _validation;

    // Data
    private FormationData _selectedFormation;
    private MaterialData _selectedMaterial;
    private ArtifactType _selectedArtifactType;
    private List<PlacedComponent> _placedComponents = new();
    private ArtifactBlueprint _currentBlueprint;

    // Dependencies
    private OpenSCADBridge _openscadBridge;
    private FormationLibrary _formationLibrary;

    // Workflow State
    public enum WorkflowState
    {
        SelectFormation,
        SelectMaterial,
        DesignArtifact,
        CompileMesh,
        ValidateBlueprint,
        ForgeArtifact
    }

    private WorkflowState _currentState = WorkflowState.SelectFormation;

    public override void _Ready()
    {
        _initialize_references();
        _connect_signals();
    }

    private void _initialize_references()
    {
        // Get UI components
        _formationLibrary = GetNode<FormationLibraryPanel>("%FormationLibrary");
        _materialSelector = GetNode<MaterialSelectorPanel>("%MaterialSelector");
        _viewport = GetNode<ArtifactViewport>("%Viewport");
        _palette = GetNode<PrefabPalette>("%PrefabPalette");
        _codeView = GetNode<OpenSCADCodeView>("%CodeView");
        _validation = GetNode<ValidationPanel>("%Validation");

        // Get managers
        _openscadBridge = OpenSCADBridge.Instance;
        _formationLibrary = FormationLibrary.Instance;
    }

    private void _connect_signals()
    {
        _formationLibrary.formation_selected += _on_formation_selected;
        _materialSelector.material_selected += _on_material_selected;
        _viewport.component_placed += _on_component_placed;
        _palette.prefab_selected += _on_prefab_selected;
    }

    private void _on_formation_selected(FormationData formation)
    {
        _selectedFormation = formation;

        // Analyze formation
        var analysis = _analyze_formation(formation);

        // Update UI
        _update_foration_info(analysis);
        _update_suggested_artifacts(analysis);

        // Enable material selection
        _materialSelector.SetEnabled(true);
        _advance_state(WorkflowState.SelectMaterial);
    }

    private void _on_material_selected(MaterialData material)
    {
        _selectedMaterial = material;

        // Validate against formation
        var validation = _validate_material_compatibility(
            _selectedFormation,
            _selectedMaterial
        );

        if (!validation.IsValid)
        {
            _show_warning(validation.WarningMessage);
            return;
        }

        // Enable artifact type selection
        _show_artifact_type_dialog();
        _advance_state(WorkflowState.DesignArtifact);
    }

    private void _on_component_placed(ComponentData component)
    {
        _placedComponents.Add(new PlacedComponent
        {
            prefab = component.Prefab,
            position = component.Position,
            rotation = component.Rotation,
            parameters = component.Parameters
        });

        // Update requirements checklist
        _update_validation();

        // Generate OpenSCAD code
        _generate_openscad_code();
    }

    private void _generate_openscad_code()
    {
        var generator = new OpenSCADGenerator();
        generator.AddInclude("std/prefabs.scad");

        // Header
        generator.AddComment($"Artifact: {_selectedArtifactType.Name}");
        generator.AddComment($"Material: {_selectedMaterial.Name}");
        generator.AddComment($"Formation: {_selectedFormation.Name}");
        generator.AddBlankLine();

        // Main module
        var moduleName = _to_snake_case(_selectedArtifactType.Name);
        generator.StartModule($"artifact_{moduleName}");

        // Add placed components
        foreach (var comp in _placedComponents)
        {
            generator.AddPrefabCall(
                comp.prefab.ModuleName,
                comp.position,
                comp.parameters
            );
        }

        // Add component mounts
        foreach (var comp in _placedComponents)
        {
            if (comp.prefab.HasMountPoint)
            {
                generator.AddComponentMount(
                    comp.prefab.MountType,
                    comp.position
                );
            }
        }

        // Add engraving zones
        generator.AddEngravingZone(
            "main",
            Vector3.Zero,
            _selectedArtifactType.Size
        );

        generator.EndModule();

        // Call module
        generator.CallModule($"artifact_{moduleName}");

        // Update code view
        _codeView.SetScript(generator.ToString());
    }

    public async void OnCompileClicked()
    {
        if (_placedComponents.Count == 0)
        {
            _show_error("No components placed");
            return;
        }

        var script = _codeView.GetScript();

        // Show loading indicator
        _set_loading(true, "Compiling artifact mesh...");

        // Compile via OpenSCAD
        var result = await _openscadBridge.CompileAsync(
            script,
            _selectedMaterial.Tier
        );

        _set_loading(false);

        if (!result.Success)
        {
            _show_error("Compilation failed: " + result.ErrorLog);
            return;
        }

        // Load mesh into viewport
        _viewport.LoadMesh(result.MeshPath);

        // Update validation
        _validation.update_status(
            "Polygons",
            $"{result.PolygonCount} / {_get_polygon_budget()}",
            result.PolygonCount <= _get_polygon_budget()
        );

        _advance_state(WorkflowState.ValidateBlueprint);
    }

    public async void OnForgeArtifactClicked()
    {
        // Final validation
        var validation = _validate_complete_blueprint();
        if (!validation.IsValid)
        {
            _show_error("Cannot forge: " + validation.ErrorMessage);
            return;
        }

        // Show confirmation dialog
        var confirmed = await _show_forge_confirmation_dialog();
        if (!confirmed)
            return;

        // Create blueprint
        _currentBlueprint = _assemble_blueprint();

        // Forge artifact
        var artifact = await _forge_artifact(_currentBlueprint);

        // Success!
        _show_success_dialog(artifact);

        // Save and clean up
        _save_blueprint(_currentBlueprint);
        _cleanup_for_next_artifact();
    }

    // Helper methods...
}
```

---

## Implementation Checklist

### Phase 1: Scene Structure (Week 1)
- [ ] Create ArtifactForgeScene.tscn
- [ ] Set up basic layout with panels
- [ ] Create background sect interior
- [ ] Add return button functionality

### Phase 2: Formation & Material Selection (Week 1-2)
- [ ] Implement FormationLibraryPanel
- [ ] Load formations from save data
- [ ] Implement MaterialSelectorPanel
- [ ] Connect to MaterialDatabase
- [ ] Add validation warnings

### Phase 3: 3D Viewport (Week 2)
- [ ] Create SubViewport with 3D scene
- [ ] Add camera controls (rotate, pan, zoom)
- [ ] Add grid helper
- [ ] Add component markers

### Phase 4: Prefab Palette (Week 2-3)
- [ ] Create PrefabPalette UI
- [ ] Load prefabs from library
- [ ] Implement drag-and-drop
- [ ] Add parameter editing

### Phase 5: OpenSCAD Integration (Week 3-4)
- [ ] Implement OpenSCADBridge.cs
- [ ] Implement OBJParser.cs
- [ ] Connect compilation to viewport
- [ ] Add error handling

### Phase 6: Validation & Forging (Week 4)
- [ ] Implement ValidationPanel
- [ ] Add requirement tracking
- [ ] Implement final validation
- [ ] Create ArtifactInstance
- [ ] Add success/failure dialogs

### Phase 7: Polish (Week 5)
- [ ] Add animations
- [ ] Add sound effects
- [ ] Add tooltips
- [ ] Tutorial integration

---

## Open Questions

- [ ] Undo/Redo support for modeling?
- [ ] Can players save artifact designs as blueprints before forging?
- [ ] How to show heat generation visually?
- [ ] Multiplayer: Can players share blueprints?
- [ ] Should there be a test mode to see artifact in action?
