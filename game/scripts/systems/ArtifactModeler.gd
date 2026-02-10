class_name ArtifactModeler extends Node3D

## The main controller for the Artifact Modeler interface.
## Similar to EngravingTable but adapted for 3D artifact modeling.

# Signals
signal primitive_placed(primitive: ArtifactPrimitive)
signal primitive_selected(primitive: ArtifactPrimitive)
signal compilation_started
signal compilation_finished(success: bool, mesh: ArrayMesh)

# Tool modes
enum ToolMode { SELECT, PLACE, ROTATE, SCALE }
var current_tool: ToolMode = ToolMode.SELECT

# State
var grid_system: GridSystem3D
var placed_primitives: Dictionary = {}  # id -> {data: ArtifactPrimitive, visual: MeshInstance3D}
var selected_primitive_id: String = ""
var primitive_counter: int = 0

# Current placement template
var current_primitive_template: ArtifactPrimitive = null

# Components
@onready var camera_controller = $CameraController
@onready var primitives_layer = $PrimitivesLayer
@onready var compiled_mesh_layer = $CompiledMeshLayer
@onready var ghost_preview = $GhostPreview
@onready var selection_indicator = $SelectionIndicator
@onready var grid_mesh = $Grid3D
@onready var cursor_3d = $Cursor3D

# UI References
var palette_panel: PanelContainer
var props_panel_ctrl: PanelContainer
var editor_toolbar: ModelerToolbar
@onready var openscad_bridge = $OpenSCADBridge

# Grid settings
@export var grid_extents: Vector3i = Vector3i(20, 20, 20)
@export var cell_size: float = 1.0

func _ready() -> void:
	print("ArtifactModeler initialized")
	_setup_grid()
	_setup_camera()
	_setup_ui()
	_connect_signals()

func _setup_grid() -> void:
	# Initialize the grid system
	grid_system = GridSystem3D.new(
		Vector3(cell_size, cell_size, cell_size),
		grid_extents,
		Vector3.ZERO
	)

	# Create grid visualization mesh
	if grid_mesh:
		grid_mesh.mesh = grid_system.create_grid_mesh()

func _setup_camera() -> void:
	if camera_controller:
		# Get the internal camera node
		var camera = camera_controller.get_node("Camera") as Camera3D
		if camera:
			camera.current = true

func _setup_ui() -> void:
	# Create a CanvasLayer to hold UI
	var ui_layer = CanvasLayer.new()
	add_child(ui_layer)

	# Instantiate Toolbar (Top)
	var toolbar_scene = preload("res://scenes/ui/ModelerToolbar.tscn")
	editor_toolbar = toolbar_scene.instantiate()
	ui_layer.add_child(editor_toolbar)
	editor_toolbar.set_anchors_preset(Control.PRESET_TOP_WIDE)

	# Create Palette (Left) programmatically
	var palette_script = load("res://scripts/ui/PrimitivePalette.gd")
	palette_panel = PanelContainer.new()
	palette_panel.set_script(palette_script.duplicate())
	palette_panel.set_anchors_preset(Control.PRESET_TOP_LEFT)
	palette_panel.position = Vector2(10, 60)
	palette_panel.custom_minimum_size = Vector2(190, 400)
	ui_layer.add_child(palette_panel)
	palette_panel.visible = false

	# Create Properties Panel (Right) programmatically
	var props_script = load("res://scripts/ui/PropertiesPanel3D.gd")
	props_panel_ctrl = PanelContainer.new()
	props_panel_ctrl.set_script(props_script.duplicate())
	props_panel_ctrl.set_anchors_preset(Control.PRESET_TOP_LEFT)
	props_panel_ctrl.custom_minimum_size = Vector2(200, 400)
	ui_layer.add_child(props_panel_ctrl)
	props_panel_ctrl.visible = false

	# Position right panel after scene tree is ready
	call_deferred("_position_right_panel")

func _connect_signals() -> void:
	if editor_toolbar:
		editor_toolbar.tool_changed.connect(_on_tool_changed)
		editor_toolbar.compile_pressed.connect(_on_compile_pressed)

	if palette_panel:
		palette_panel.primitive_selected.connect(_on_primitive_selected)

	if props_panel_ctrl:
		props_panel_ctrl.transform_changed.connect(_on_transform_changed)
		props_panel_ctrl.parameter_changed.connect(_on_parameter_changed)
		props_panel_ctrl.csg_changed.connect(_on_csg_changed)
		props_panel_ctrl.delete_requested.connect(_on_delete_requested)

func _position_right_panel() -> void:
	if props_panel_ctrl:
		var viewport_size = get_viewport().get_visible_rect().size
		props_panel_ctrl.position = Vector2(viewport_size.x - 210, 60)

func _unhandled_input(event: InputEvent) -> void:
	# Check if mouse is over UI
	var mouse_pos = get_viewport().get_mouse_position()
	if _is_mouse_over_ui(mouse_pos):
		return

	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			_handle_left_click()
		elif event.button_index == MOUSE_BUTTON_RIGHT and event.pressed:
			_handle_right_click()

func _is_mouse_over_ui(mouse_pos: Vector2) -> bool:
	if palette_panel and palette_panel.visible and palette_panel.get_global_rect().has_point(mouse_pos):
		return true
	if props_panel_ctrl and props_panel_ctrl.visible and props_panel_ctrl.get_global_rect().has_point(mouse_pos):
		return true
	if editor_toolbar and editor_toolbar.get_global_rect().has_point(mouse_pos):
		return true
	return false

func _handle_left_click() -> void:
	var camera = camera_controller.get_node("Camera") as Camera3D
	if not camera:
		return

	var mouse_pos = get_viewport().get_mouse_position()
	var grid_pos = grid_system.raycast_to_grid(camera, mouse_pos)

	# Check for invalid position (sentinel value)
	if grid_pos.x < -99990 or grid_pos.y < -99990 or grid_pos.z < -99990:
		return

	match current_tool:
		ToolMode.PLACE:
			if current_primitive_template:
				_place_primitive(current_primitive_template, grid_pos)
		ToolMode.SELECT:
			_select_primitive_at(grid_pos)

func _handle_right_click() -> void:
	# Could be used for rotation context menu or cancel current action
	if current_tool == ToolMode.PLACE:
		current_primitive_template = null
		_update_ghost_visibility()

func _place_primitive(template: ArtifactPrimitive, grid_pos: Vector3i) -> void:
	# Generate unique ID
	primitive_counter += 1
	var new_id = "prim_%d" % primitive_counter

	# Create new primitive from template
	var new_prim = template.duplicate_with_id(new_id)
	new_prim.position = grid_pos

	# Create visual mesh
	var visual = MeshInstance3D.new()
	visual.name = new_id
	visual.mesh = _create_mesh_for_primitive(new_prim)
	visual.position = grid_system.grid_to_world(grid_pos)
	visual.rotation_degrees = new_prim.rotation
	visual.scale = new_prim.scale

	# Set material with color based on CSG operation
	var material = StandardMaterial3D.new()
	material.albedo_color = new_prim.color
	material.metallic = 0.3
	material.roughness = 0.6
	material.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
	material.alpha_scissor_threshold = 0.5
	visual.material_override = material

	primitives_layer.add_child(visual)

	# Store primitive data
	placed_primitives[new_id] = {
		"data": new_prim,
		"visual": visual
	}

	print("Placed primitive: %s at %s" % [new_prim.display_name, grid_pos])
	emit_signal("primitive_placed", new_prim)

func _select_primitive_at(grid_pos: Vector3i) -> void:
	# Find primitive at this grid position
	var found_id = ""

	for prim_id in placed_primitives:
		var entry = placed_primitives[prim_id]
		var prim = entry["data"] as ArtifactPrimitive
		if prim.position == grid_pos:
			found_id = prim_id
			break

	if found_id.is_empty():
		_deselect_all()
		return

	_select_primitive(found_id)

func _select_primitive(prim_id: String) -> void:
	if not placed_primitives.has(prim_id):
		return

	selected_primitive_id = prim_id
	var entry = placed_primitives[prim_id]
	var prim = entry["data"] as ArtifactPrimitive
	var visual = entry["visual"] as MeshInstance3D

	# Update selection indicator
	if selection_indicator:
		selection_indicator.position = visual.position
		selection_indicator.mesh = visual.mesh
		selection_indicator.rotation_degrees = prim.rotation
		selection_indicator.scale = prim.scale * 1.1  # Slightly larger
		selection_indicator.visible = true

	# Show properties panel
	if props_panel_ctrl:
		var camera = camera_controller.get_node("Camera") as Camera3D
		props_panel_ctrl.setup(prim, visual, camera)

	emit_signal("primitive_selected", prim)

func _deselect_all() -> void:
	selected_primitive_id = ""
	if selection_indicator:
		selection_indicator.visible = false
	if props_panel_ctrl:
		props_panel_ctrl.visible = false

func _create_mesh_for_primitive(prim: ArtifactPrimitive) -> Mesh:
	match prim.type:
		ArtifactPrimitive.PrimitiveType.CUBE:
			return _create_cube_mesh(prim.size)
		ArtifactPrimitive.PrimitiveType.SPHERE:
			return _create_sphere_mesh(prim.radius)
		ArtifactPrimitive.PrimitiveType.CYLINDER:
			return _create_cylinder_mesh(prim.radius, prim.height)
		ArtifactPrimitive.PrimitiveType.CONE:
			return _create_cone_mesh(prim.radius, prim.height)
		ArtifactPrimitive.PrimitiveType.TORUS:
			return _create_torus_mesh(prim.radius, prim.tube_radius)
		_:
			return _create_cube_mesh(Vector3(1, 1, 1))

func _create_cube_mesh(size: Vector3) -> ArrayMesh:
	var mesh = ArrayMesh.new()
	var st = SurfaceTool.new()
	st.begin(Mesh.PRIMITIVE_TRIANGLES)

	var hs = size / 2.0

	# Define 6 faces of cube
	var faces = [
		# Front
		[Vector3(-hs.x, -hs.y, hs.z), Vector3(hs.x, -hs.y, hs.z), Vector3(hs.x, hs.y, hs.z), Vector3(-hs.x, hs.y, hs.z)],
		# Back
		[Vector3(hs.x, -hs.y, -hs.z), Vector3(-hs.x, -hs.y, -hs.z), Vector3(-hs.x, hs.y, -hs.z), Vector3(hs.x, hs.y, -hs.z)],
		# Top
		[Vector3(-hs.x, hs.y, hs.z), Vector3(hs.x, hs.y, hs.z), Vector3(hs.x, hs.y, -hs.z), Vector3(-hs.x, hs.y, -hs.z)],
		# Bottom
		[Vector3(-hs.x, -hs.y, -hs.z), Vector3(hs.x, -hs.y, -hs.z), Vector3(hs.x, -hs.y, hs.z), Vector3(-hs.x, -hs.y, hs.z)],
		# Right
		[Vector3(hs.x, -hs.y, hs.z), Vector3(hs.x, -hs.y, -hs.z), Vector3(hs.x, hs.y, -hs.z), Vector3(hs.x, hs.y, hs.z)],
		# Left
		[Vector3(-hs.x, -hs.y, -hs.z), Vector3(-hs.x, -hs.y, hs.z), Vector3(-hs.x, hs.y, hs.z), Vector3(-hs.x, hs.y, -hs.z)]
	]

	var normals = [
		Vector3.FORWARD, Vector3.BACK, Vector3.UP, Vector3.DOWN, Vector3.RIGHT, Vector3.LEFT
	]

	for i in range(6):
		var verts = faces[i]
		var normal = normals[i]
		st.add_vertex(verts[0])
		st.add_vertex(verts[1])
		st.add_vertex(verts[2])
		st.add_vertex(verts[0])
		st.add_vertex(verts[2])
		st.add_vertex(verts[3])

	st.generate_normals()
	return st.commit()

func _create_sphere_mesh(radius: float) -> Mesh:
	var sphere = SphereMesh.new()
	sphere.radius = radius
	sphere.height = radius * 2
	sphere.radial_segments = 32
	sphere.rings = 16
	return sphere

func _create_cylinder_mesh(radius: float, height: float) -> Mesh:
	var cylinder = CylinderMesh.new()
	cylinder.top_radius = radius
	cylinder.bottom_radius = radius
	cylinder.height = height
	cylinder.radial_segments = 32
	return cylinder

func _create_cone_mesh(radius: float, height: float) -> Mesh:
	var cylinder = CylinderMesh.new()
	cylinder.top_radius = 0
	cylinder.bottom_radius = radius
	cylinder.height = height
	cylinder.radial_segments = 32
	return cylinder

func _create_torus_mesh(radius: float, tube_radius: float) -> Mesh:
	var torus = TorusMesh.new()
	torus.radius = radius
	torus.inner_radius = radius - tube_radius
	torus.rings = 32
	torus.radial_segments = 32
	return torus

func _update_ghost_visibility() -> void:
	if ghost_preview:
		ghost_preview.visible = current_primitive_template != null
		if current_primitive_template:
			ghost_preview.mesh = _create_mesh_for_primitive(current_primitive_template)

func _on_tool_changed(tool_name: String) -> void:
	print("Switched tool to: ", tool_name)
	match tool_name:
		"SELECT":
			current_tool = ToolMode.SELECT
			palette_panel.visible = false
			_deselect_all()
		"PLACE":
			current_tool = ToolMode.PLACE
			palette_panel.visible = true
			props_panel_ctrl.visible = false
		"ROTATE":
			current_tool = ToolMode.ROTATE
			palette_panel.visible = false
		"SCALE":
			current_tool = ToolMode.SCALE
			palette_panel.visible = false

func _on_primitive_selected(primitive: ArtifactPrimitive) -> void:
	print("Selected primitive for placement: ", primitive.display_name)
	current_primitive_template = primitive
	current_tool = ToolMode.PLACE
	_update_ghost_visibility()

func _on_transform_changed(prim_id: String, property: String, value: Variant) -> void:
	if not placed_primitives.has(prim_id):
		return

	var entry = placed_primitives[prim_id]
	var prim = entry["data"]
	var visual = entry["visual"]

	match property:
		"position":
			prim.position = value
			visual.position = grid_system.grid_to_world(value)
		"rotation":
			prim.rotation = value
			visual.rotation_degrees = value
		"scale":
			prim.scale = value
			visual.scale = value

func _on_parameter_changed(prim_id: String, parameter: String, value: Variant) -> void:
	if not placed_primitives.has(prim_id):
		return

	var entry = placed_primitives[prim_id]
	var prim = entry["data"]
	var visual = entry["visual"]

	match parameter:
		"size":
			prim.size = value
		"radius":
			prim.radius = value
		"height":
			prim.height = value
		"tube_radius":
			prim.tube_radius = value

	# Re-create mesh with new parameters
	visual.mesh = _create_mesh_for_primitive(prim)

func _on_csg_changed(prim_id: String, operation: int, parent_id: String) -> void:
	if not placed_primitives.has(prim_id):
		return

	var entry = placed_primitives[prim_id]
	var prim = entry["data"]

	prim.csg_op = operation
	prim.parent_id = parent_id

	# Update visual material color
	var material = StandardMaterial3D.new()
	material.albedo_color = prim.get_csg_color()
	material.metallic = 0.3
	material.roughness = 0.6
	entry["visual"].material_override = material

func _on_delete_requested(prim_id: String) -> void:
	if not placed_primitives.has(prim_id):
		return

	var entry = placed_primitives[prim_id]
	var visual = entry["visual"]

	# Remove visual
	if is_instance_valid(visual):
		visual.queue_free()

	# Remove from dictionary
	placed_primitives.erase(prim_id)

	# Deselect if this was selected
	if selected_primitive_id == prim_id:
		_deselect_all()

	print("Deleted primitive: ", prim_id)

func _on_compile_pressed() -> void:
	print("Compile button pressed")

	if placed_primitives.is_empty():
		editor_toolbar.set_result(false, "No primitives to compile")
		return

	# Update UI
	editor_toolbar.set_compiling(true)
	emit_signal("compilation_started")

	# Generate OpenSCAD script
	var generator = OpenSCADGenerator.new()
	var script = generator.generate(placed_primitives)
	print("Generated OpenSCAD script:\n", script)

	# Call C# compilation
	if openscad_bridge:
		_compile_async(script)
	else:
		editor_toolbar.set_compiling(false)
		editor_toolbar.set_result(false, "OpenSCADBridge not initialized")

func _compile_async(script: String) -> void:
	# This needs to be called via the C# bridge
	# For now, we'll use a simple approach that assumes the bridge exists
	openscad_bridge.call_deferred("CompileAndDisplay", script, editor_toolbar, self)

## Display the compiled mesh in the viewport
func _display_compiled_mesh(mesh: ArrayMesh) -> void:
	# Clear any previous compiled mesh
	for child in compiled_mesh_layer.get_children():
		child.queue_free()

	# Create a mesh instance to display the result
	var display = MeshInstance3D.new()
	display.name = "CompiledArtifact"
	display.mesh = mesh

	# Add material
	var material = StandardMaterial3D.new()
	material.albedo_color = Color(0.8, 0.7, 0.6)
	material.metallic = 0.4
	material.roughness = 0.5
	display.material_override = material

	# Center the mesh
	var aabb = mesh.get_aabb()
	var center = aabb.position + aabb.size / 2
	display.position = -center

	# Add to compiled mesh layer
	compiled_mesh_layer.add_child(display)

	print("Compiled mesh displayed with ", mesh.get_surface_count(), " surfaces")
