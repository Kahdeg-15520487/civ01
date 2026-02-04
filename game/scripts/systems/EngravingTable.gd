class_name EngravingTable
extends Node2D

## The main "IDE" controller for the Rune Engraving interface.

# Signals
signal simulation_started
signal simulation_stopped
signal rune_placed(rune_instance)

# State
var is_simulating: bool = false
var current_board_state: Dictionary = {} # Stores placed runes and traces

# Grid Settings
@export var grid_size: int = 20 # Larger grid for readability (Was 10)
@export var grid_width: int = 80 # 80 * 10 = 800px (Was 20)
@export var grid_height: int = 56 # 56 * 10 = 560px (Was 14)

var grid_system: GridSystem

var selected_rune_type: Rune = null # The rune currently selected in palette
var rune_palette_ui: RunePalette

# Layers
var trace_layer: Node2D
var rune_layer: Node2D

func _ready() -> void:
	print("Engraving Table Initialized")
	_setup_layers()
	_setup_grid()
	_setup_camera()
	_setup_ui()
	_setup_simulation()

func _setup_camera() -> void:
	camera = Camera2D.new()
	camera.enabled = true
	# Center camera on grid initially
	var center_x = (grid_width * grid_size) / 2.0
	var center_y = (grid_height * grid_size) / 2.0
	camera.position = Vector2(center_x, center_y) + Vector2(180, 50) # + offset
	add_child(camera)

func _reset_camera() -> void:
	var center_x = (grid_width * grid_size) / 2.0
	var center_y = (grid_height * grid_size) / 2.0
	camera.position = Vector2(center_x, center_y) + Vector2(180, 50)
	zoom_level = 1.0
	camera.zoom = Vector2(1, 1)

func _adjust_zoom(factor: float) -> void:
	zoom_level *= factor
	zoom_level = clamp(zoom_level, MIN_ZOOM, MAX_ZOOM)
	camera.zoom = Vector2(zoom_level, zoom_level)


func _setup_layers() -> void:
	# Define rendering order: 
	# 1. Grid (drawn by self via _draw)
	# 2. Traces
	# 3. Runes
	trace_layer = Node2D.new()
	add_child(trace_layer)
	
	rune_layer = Node2D.new()
	add_child(rune_layer)


func _setup_grid() -> void:
	# Initialize the grid system logic
	# Sidebar is 150px wide, so we start grid at 180 to give some padding.
	grid_system = GridSystem.new(Vector2(grid_size, grid_size), grid_width, grid_height, Vector2(180, 50))
	queue_redraw() # Trigger _draw


func _on_rune_selected(rune: Rune) -> void:
	print("Selected rune for placement: ", rune.display_name)
	selected_rune_type = rune

var camera: Camera2D
var zoom_level: float = 1.0
const MIN_ZOOM: float = 0.5
const MAX_ZOOM: float = 5.0
const ZOOM_SPEED: float = 0.1

func _input(event: InputEvent) -> void:
	# Camera Controls
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			_adjust_zoom(1.0 + ZOOM_SPEED)
			return
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			_adjust_zoom(1.0 - ZOOM_SPEED)
			return
		elif event.button_index == MOUSE_BUTTON_MIDDLE:
			if event.pressed:
				# Start Pan
				pass
	
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_SPACE:
			_reset_camera()

	if event is InputEventMouseMotion:
		if Input.is_mouse_button_pressed(MOUSE_BUTTON_MIDDLE):
			camera.position -= event.relative / zoom_level
			
		queue_redraw()
		if is_dragging_trace and current_trace_visual:
			var mouse_pos = get_global_mouse_position()
			var grid_pos = grid_system.world_to_grid(mouse_pos)
			
			if grid_system.is_valid_pos(grid_pos):
				# Orthogonal Snapping Logic (Manhattan)
				# 1. Start point
				var start_world = grid_system.grid_to_world(drag_start_grid)
				# 2. End point (Grid center)
				var end_world = grid_system.grid_to_world(grid_pos)
				
				# 3. Calculate intermediate point (Elbow)
				# Heuristic: Preserve dragging direction? Or simple L-shape.
				# Let's do simple X-then-Y for now.
				var elbow_point = Vector2(end_world.x, start_world.y)
				
				# Update visual with 3 points
				# Note: Need to update QiTraceVisual to handle multiple points
				current_trace_visual.update_path([start_world, elbow_point, end_world])
			else:
				# Free drag (fallback)
				current_trace_visual.update_endpoint(mouse_pos)


	elif event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT:
			if event.pressed:
				_handle_click(get_global_mouse_position())
				
		elif event.button_index == MOUSE_BUTTON_RIGHT:
			# Right click to draw traces ONLY if in Route mode
			if current_tool == ToolMode.DRAW_TRACE:
				if event.pressed:
					_start_trace(get_global_mouse_position())
				else:
					_end_trace(get_global_mouse_position())
			elif current_tool == ToolMode.PLACE_RUNE:
				# Maybe clear selection or cancel placement?
				pass


func _start_trace(screen_pos: Vector2) -> void:
	var grid_pos = grid_system.world_to_grid(screen_pos)
	if not grid_system.is_valid_pos(grid_pos): return
	
	is_dragging_trace = true
	drag_start_grid = grid_pos
	
	# Create Data
	var trace_data = QiTrace.new()
	trace_data.width = trace_widths[current_trace_width_idx]
	trace_data.start_point = grid_system.grid_to_world(grid_pos)

	trace_data.end_point = trace_data.start_point # Initially same
	
	# Create Visual
	current_trace_visual = QiTraceVisualScene.instantiate()
	trace_layer.add_child(current_trace_visual)
	current_trace_visual.setup(trace_data)

	
	print("Started drawing trace from: ", grid_pos)

var placed_traces: Array[Dictionary] = [] # { "data": QiTrace, "visual": QiTraceVisual }

func _end_trace(screen_pos: Vector2) -> void:
	if not is_dragging_trace: return
	
	var grid_pos = grid_system.world_to_grid(screen_pos)
	print("Ended trace at: ", grid_pos)
	
	# Store the valid trace
	placed_traces.append({
		"data": current_trace_visual.trace_data, # Assuming visual stores data
		"visual": current_trace_visual
	})
	
	is_dragging_trace = false
	current_trace_visual = null


func _handle_click(screen_pos: Vector2) -> void:
	var grid_pos = grid_system.world_to_grid(screen_pos)
	if grid_system.is_valid_pos(grid_pos):
		print("Clicked on cell: ", grid_pos)
		
		match current_tool:
			ToolMode.PLACE_RUNE:
				if selected_rune_type:
					place_rune(selected_rune_type, grid_pos)
				else:
					print("No rune selected!")
			ToolMode.SELECT:
				_select_entity_at(grid_pos)
			ToolMode.DRAW_TRACE:
				pass # Left click in draw mode does nothing (or maybe selects?)


## Checks if a rectangular area on the grid is clear
func is_area_clear(center_grid: Vector2i, size_in_cells: Vector2i) -> bool:
	var top_left = center_grid - (size_in_cells / 2)
	for x in range(size_in_cells.x):
		for y in range(size_in_cells.y):
			var check_pos = top_left + Vector2i(x, y)
			if not grid_system.is_valid_pos(check_pos): return false
			if current_board_state.has(check_pos): return false
	return true

## Called when user drags a rune onto the board
func place_rune(rune_resource: Rune, grid_position: Vector2i) -> void:
	if not is_area_clear(grid_position, rune_resource.size_in_cells):
		print("Area blocked!")
		return
		
	print("Placing rune: %s at %s" % [rune_resource.display_name, grid_position])
	
	var visual = RuneVisualScene.instantiate()
	rune_layer.add_child(visual)
	visual.setup(rune_resource)
	visual.position = grid_system.grid_to_world(grid_position)
	
	# Mark all cells as occupied
	var top_left = grid_position - (rune_resource.size_in_cells / 2)
	for x in range(rune_resource.size_in_cells.x):
		for y in range(rune_resource.size_in_cells.y):
			var occupied_pos = top_left + Vector2i(x, y)
			current_board_state[occupied_pos] = {
				"rune": rune_resource,
				"visual": visual,
				"main_pos": grid_position # Reference to center
			}

func _draw() -> void:
	if grid_system:
		grid_system.draw_grid(self )
		
		# Draw Cursor Highlight
		var mouse_pos = get_global_mouse_position()
		var grid_pos = grid_system.world_to_grid(mouse_pos)
		
		if grid_system.is_valid_pos(grid_pos):
			var snap_pos = grid_system.grid_to_world(grid_pos)
			
			# Determine size of highlight based on selected rune or default 1x1
			var highlight_size_cells = Vector2i(1, 1)
			if current_tool == ToolMode.PLACE_RUNE and selected_rune_type:
				highlight_size_cells = selected_rune_type.size_in_cells
			
			var rect_size = Vector2(highlight_size_cells) * grid_system.cell_size.x # Assumes square cells
			var rect = Rect2(snap_pos - rect_size / 2.0, rect_size)
			
			var color = Color(1, 1, 0, 0.3)
			if current_tool == ToolMode.PLACE_RUNE and selected_rune_type:
				if not is_area_clear(grid_pos, highlight_size_cells):
					color = Color(1, 0, 0, 0.3) # Red if blocked
			
			draw_rect(rect, color, true) # Highlight
			draw_rect(rect, color.lightened(0.5), false, 2.0) # Border


const RuneVisualScene = preload("res://scenes/systems/RuneVisual.tscn")
const QiTraceVisualScene = preload("res://scenes/systems/QiTraceVisual.tscn")
const NetlistExtractorScript = preload("res://scripts/systems/NetlistExtractor.gd")

# State
enum ToolMode {PLACE_RUNE, DRAW_TRACE, SELECT}
var current_tool: ToolMode = ToolMode.SELECT # Default to Select
var is_dragging_trace: bool = false
var current_trace_visual: QiTraceVisual = null
var drag_start_grid: Vector2i

# Trace Settings
var current_trace_width_idx: int = 0
var trace_widths: Array[float] = [10.0, 40.0, 120.0] # Small (1), Medium (4), Large (12) * 10px

var properties_panel: PropertiesPanel
var editor_toolbar: EditorToolbar
var selected_entity_pos: Vector2i = Vector2i(-1, -1)


func _setup_ui() -> void:
	# Create a CanvasLayer to hold UI so it stays above the game world
	var ui_layer = CanvasLayer.new()
	add_child(ui_layer)
	
	# Instantiate Toolbar (Top)
	var toolbar_scene = preload("res://scenes/ui/EditorToolbar.tscn")
	editor_toolbar = toolbar_scene.instantiate()
	ui_layer.add_child(editor_toolbar)
	
	# Connect Toolbar Signals
	editor_toolbar.tool_changed.connect(_on_tool_changed)
	editor_toolbar.trace_width_changed.connect(_on_trace_width_changed)
	editor_toolbar.request_start_sim.connect(start_simulation)
	editor_toolbar.request_stop_sim.connect(stop_simulation)
	
	# Instantiate Palette (Left)
	var palette_scene = preload("res://scenes/ui/RunePalette.tscn")
	rune_palette_ui = palette_scene.instantiate()
	ui_layer.add_child(rune_palette_ui)
	
	# Anchor to Left, Top (below toolbar), Bottom
	rune_palette_ui.set_anchors_preset(Control.PRESET_LEFT_WIDE)
	rune_palette_ui.offset_top = 50 # Below toolbar
	rune_palette_ui.visible = false
	rune_palette_ui.rune_selected.connect(_on_rune_selected)
	
	# Instantiate Properties Panel (Right)
	var props_scene = preload("res://scenes/ui/PropertiesPanel.tscn")
	properties_panel = props_scene.instantiate()
	ui_layer.add_child(properties_panel)
	
	# Configure Anchors (Top Right)
	properties_panel.set_anchors_preset(Control.PRESET_TOP_RIGHT)
	properties_panel.grow_horizontal = Control.GROW_DIRECTION_BEGIN # Grow to the left
	properties_panel.position = Vector2.ZERO # Reset position to let anchors take over?
	# Actually, set_anchors_preset sets offsets too usually. 
	# Let's ensure it has a margin.
	properties_panel.offset_top = 50
	properties_panel.offset_right = -20
	properties_panel.delete_requested.connect(_on_delete_requested)
	
	# Select first one by default if available
	if rune_palette_ui.rune_categories.size() > 0:
		var first_cat_runes = rune_palette_ui.rune_categories.values()[0]
		if first_cat_runes.size() > 0:
			selected_rune_type = first_cat_runes[0]

func _on_tool_changed(tool_name: String) -> void:
	print("Switched tool to: ", tool_name)
	match tool_name:
		"SELECT":
			current_tool = ToolMode.SELECT
			rune_palette_ui.visible = false
		"PLACE":
			current_tool = ToolMode.PLACE_RUNE
			rune_palette_ui.visible = true
			properties_panel.visible = false
		"ROUTE":
			current_tool = ToolMode.DRAW_TRACE
			rune_palette_ui.visible = false
			properties_panel.visible = false

# ... (Previous code)

var selected_trace_idx: int = -1

func _select_entity_at(grid_pos: Vector2i) -> void:
	# 1. Check Runes
	if current_board_state.has(grid_pos):
		# Handle multi-cell runes: find the main entry
		var entry = current_board_state[grid_pos]
		if entry.has("main_pos"):
			grid_pos = entry["main_pos"]
			entry = current_board_state[grid_pos]
			
		var rune = entry["rune"] as Rune
		print("Selected Entity: %s" % rune.display_name)
		
		selected_entity_pos = grid_pos
		selected_trace_idx = -1
		properties_panel.setup(rune)
		return

	# 2. Check Traces
	# We check the distance from the mouse position to any trace segment
	var mouse_pos = get_global_mouse_position()
	for i in range(placed_traces.size()):
		var t = placed_traces[i]
		var visual = t["visual"] as QiTraceVisual
		var width = t["data"].width
		
		# Iterate line segments
		var points = visual.line_2d.points
		for j in range(points.size() - 1):
			var p1 = points[j]
			var p2 = points[j + 1]
			var closest = Geometry2D.get_closest_point_to_segment(mouse_pos, p1, p2)
			
			if mouse_pos.distance_to(closest) < (width / 2.0) + 5.0: # Width + tolerance
				print("Selected Trace (Index %d)" % i)
				selected_trace_idx = i
				selected_entity_pos = Vector2i(-1, -1)
				properties_panel.setup(t["data"])
				return

	# 3. Nothing selected
	print("Selected Empty Space")
	selected_entity_pos = Vector2i(-1, -1)
	selected_trace_idx = -1
	properties_panel.clear()


func _on_delete_requested() -> void:
	if selected_entity_pos != Vector2i(-1, -1):
		_delete_rune_at(selected_entity_pos)
		selected_entity_pos = Vector2i(-1, -1)
		properties_panel.clear()
	elif selected_trace_idx != -1:
		_delete_trace_at(selected_trace_idx)
		selected_trace_idx = -1
		properties_panel.clear()

func _delete_trace_at(idx: int) -> void:
	if idx < 0 or idx >= placed_traces.size(): return
	
	var t = placed_traces[idx]
	var visual = t["visual"]
	if is_instance_valid(visual):
		visual.queue_free()
	
	placed_traces.remove_at(idx)
	print("Deleted trace at index ", idx)


func _delete_rune_at(grid_pos: Vector2i) -> void:
	if not current_board_state.has(grid_pos): return
	
	var entry = current_board_state[grid_pos]
	var rune = entry["rune"] as Rune
	var visual = entry["visual"]
	
	# Remove Visual
	if is_instance_valid(visual):
		visual.queue_free()
		
	# Clear grid cells
	var top_left = grid_pos - (rune.size_in_cells / 2)
	for x in range(rune.size_in_cells.x):
		for y in range(rune.size_in_cells.y):
			var occupied_pos = top_left + Vector2i(x, y)
			current_board_state.erase(occupied_pos)
			
	print("Deleted rune at ", grid_pos)


func _on_trace_width_changed(width_idx: int) -> void:
	current_trace_width_idx = width_idx
	print("Trace width set to: ", trace_widths[current_trace_width_idx])


@onready var sim_manager_cs: Node = null # The C# bridge node


func _setup_simulation() -> void:
	# Instantiate the C# Simulation Manager
	# Assuming SimulationManager.cs is a GlobalClass "SimulationManager"
	# But in C# GlobalClasses might need full path or manual loading if not appearing in ClassDB yet.
	# Let's try loading by resource path to be safe.
	var sim_script = load("res://scripts/emulator/SimulationManager.cs")
	if sim_script:
		sim_manager_cs = Node.new()
		sim_manager_cs.set_script(sim_script)
		sim_manager_cs.name = "SimulationManager"
		add_child(sim_manager_cs)
		print("SimulationManager C# Node Added")
	else:
		push_error("Failed to load SimulationManager.cs")

## Starts the Chi flow simulation
func start_simulation() -> void:
	if is_simulating: return
	
	print("Starting Simulation...")
	is_simulating = true
	emit_signal("simulation_started")
	
	# 1. Extract Netlist
	var extractor = NetlistExtractorScript.new(grid_system)
	var graph_data = extractor.extract(current_board_state, placed_traces)
	
	# 2. Build Graph in C#
	if sim_manager_cs:
		sim_manager_cs.call("BuildGraph", graph_data)
		
	# 3. Start Tick Loop
	_run_tick()

func stop_simulation() -> void:
	is_simulating = false
	emit_signal("simulation_stopped")

func _run_tick() -> void:
	if not is_simulating: return
	
	# print("Simulation Tick")
	if sim_manager_cs:
		sim_manager_cs.call("RunTick")
	
	# Schedule next tick
	if is_simulating:
		get_tree().create_timer(0.5).timeout.connect(_run_tick)
