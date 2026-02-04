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
@export var grid_size: int = 20 # Larger grid
@export var grid_width: int = 200 # Wider (Was 80)
@export var grid_height: int = 150 # Taller (Was 56)

var grid_system: GridSystem

var selected_rune_type: Rune = null # The rune currently selected in palette
var rune_palette_ui: RunePalette

# Layers
var trace_layer: Node2D
var rune_layer: Node2D

func _ready() -> void:
	# Force Window Scaling logic
	get_window().content_scale_mode = Window.CONTENT_SCALE_MODE_CANVAS_ITEMS
	get_window().content_scale_aspect = Window.CONTENT_SCALE_ASPECT_EXPAND
	
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

func _unhandled_input(event: InputEvent) -> void:
	# Camera Controls
	if event is InputEventMouseButton:
		# Guard: Don't zoom/pan if over UI
		var mouse_pos = get_global_mouse_position()
		# Check Palette visibility and rect
		if rune_palette_ui.visible and rune_palette_ui.get_global_rect().has_point(get_viewport().get_mouse_position()):
			return
		# Check Properties Panel visibility and rect
		if properties_panel.visible and properties_panel.get_global_rect().has_point(get_viewport().get_mouse_position()):
			return
			
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


## Checks if the cells required by a rune are clear at the given position
func is_area_clear(center_grid: Vector2i, rune: Rune) -> bool:
	var occupied = rune.get_occupied_cells()
	for offset in occupied:
		var check_pos = center_grid + offset
		if not grid_system.is_valid_pos(check_pos): return false
		if current_board_state.has(check_pos): return false
	return true

## Called when user drags a rune onto the board
func place_rune(rune_resource: Rune, grid_position: Vector2i) -> void:
	# 1. SPECIAL LOGIC: Handle "Item" placement (Stones)
	if rune_resource.id.begins_with("item_stone_"):
		if current_board_state.has(grid_position):
			var original_click_pos = grid_position
			var entry = current_board_state[grid_position]
			# Entry is now shared, no need to lookup main_pos
			
			var target_rune = entry.get("rune")
			if target_rune and (target_rune.id == "source_socket" or target_rune.id == "source_array"):
				var stone_type = rune_resource.display_name.replace(" Stone", "") # Hacky extraction or use ID mapping
				if rune_resource.id == "item_stone_fire": stone_type = "Fire"
				elif rune_resource.id == "item_stone_water": stone_type = "Water"
				elif rune_resource.id == "item_stone_wood": stone_type = "Wood"
				elif rune_resource.id == "item_stone_earth": stone_type = "Earth"
				elif rune_resource.id == "item_stone_metal": stone_type = "Metal"
				
				var params = entry.get("params", {})
				
				if target_rune.id == "source_socket":
					# Socket is technically a socket_pattern valid rune too now?
					# But let's keep simple logic for single-socket compatibility
					params["stone_type"] = stone_type
					print("Inserted %s into Socket" % stone_type)
				
				# Universal Slot Logic: Works for Array or any multi-socket rune
				elif not target_rune.socket_pattern.is_empty():
					# Map offset to slot index dynamically
					var clicked_offset = grid_position - entry.main_pos # Calculate offset from rune's anchor
					var slot_idx = target_rune.socket_pattern.find(clicked_offset)
					
					if slot_idx != -1:
						var slots = params.get("element_slots", [])
						var needed_size = target_rune.socket_pattern.size()
						
						# Ensure size matches definition
						if slots.size() < needed_size:
							slots.resize(needed_size)
							for i in range(slots.size()):
								if slots[i] == null: slots[i] = "None"
						
						slots[slot_idx] = stone_type
						params["element_slots"] = slots
						print("Inserted %s into Slot %d" % [stone_type, slot_idx])
					else:
						print("Clicked position %s is not a valid socket." % clicked_offset)
				
				else:
					# Should cover source_array if it has sockets
					pass
				
				# Commit params
				entry["params"] = params
				if entry.has("visual") and is_instance_valid(entry["visual"]):
					entry["visual"].update_state(params)
				
				return
		
		print("Stone must be placed in a Socket or Array!")
		return

	if not is_area_clear(grid_position, rune_resource):
		print("Area blocked!")
		return
		
	print("Placing rune: %s at %s" % [rune_resource.display_name, grid_position])
	
	var visual = RuneVisualScene.instantiate()
	rune_layer.add_child(visual)
	visual.setup(rune_resource, grid_system.cell_size.x)
	visual.position = grid_system.grid_to_world(grid_position)
	
	# Mark all cells as occupied
	var cells = rune_resource.get_occupied_cells()
	
	# Create a shared entry object for this rune instance
	var shared_entry = {
		"rune": rune_resource,
		"visual": visual,
		"params": {}, # Shared instance parameters
		"main_pos": grid_position # Reference to anchor (useful for calculations)
	}
	
	for offset in cells:
		var occupied_pos = grid_position + offset
		current_board_state[occupied_pos] = shared_entry

func _draw() -> void:
	if grid_system:
		grid_system.draw_grid(self )
		
		# Draw Cursor Highlight
		var mouse_pos = get_global_mouse_position()
		var grid_pos = grid_system.world_to_grid(mouse_pos)
		
		if grid_system.is_valid_pos(grid_pos):
			var snap_pos = grid_system.grid_to_world(grid_pos)
			
			var color = Color(1, 1, 0, 0.3) # Default yellow
			
			# Blue for Route Qi mode
			if current_tool == ToolMode.DRAW_TRACE:
				color = Color(0.3, 0.6, 1.0, 0.4) # Blue glow
			
			if current_tool == ToolMode.PLACE_RUNE and selected_rune_type:
				# Show custom shape highlight
				var cells = selected_rune_type.get_occupied_cells()
				if not is_area_clear(grid_pos, selected_rune_type):
					color = Color(1, 0, 0, 0.3) # Red if blocked
				
				for offset in cells:
					# Adjust for center logic
					var draw_pos = snap_pos + (Vector2(offset) * grid_system.cell_size)
					# Rect should be centered on that pos
					var c_rect = Rect2(draw_pos - grid_system.cell_size / 2.0, grid_system.cell_size)
					draw_rect(c_rect, color, true)
					draw_rect(c_rect, color.lightened(0.5), false, 2.0)
					
			else:
				# Default Cursor (single cell)
				var rect_size = Vector2(1, 1) * grid_system.cell_size.x
				var rect = Rect2(snap_pos - rect_size / 2.0, rect_size)
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
	
	# Force Anchors
	editor_toolbar.set_anchors_preset(Control.PRESET_TOP_WIDE)
	
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
	
	# Configure Anchors (None, we manual pos)
	# properties_panel.set_anchors_preset(Control.PRESET_TOP_RIGHT) # REMOVED
	properties_panel.delete_requested.connect(_on_delete_requested)
	properties_panel.visible = false # Hide by default
	
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
		# With shared_entry refactor, any occupied cell points to the full data.
		var entry = current_board_state[grid_pos]
		# No need to look up main_pos recursively anymore.
			
		var rune = entry["rune"] as Rune
		print("Selected Entity: %s" % rune.display_name)
		
		# Get the rune's anchor position (main_pos)
		var main_pos = entry["main_pos"] as Vector2i
		var main_world_pos = grid_system.grid_to_world(main_pos)
		
		# Calculate proper Boundaries relative to the RUNE'S ANCHOR (not clicked cell)
		var boundary_rects: Array[Rect2] = []
		
		var cells = rune.get_occupied_cells()
		var cell_size = grid_system.cell_size.x # Assuming square
		
		for offset in cells:
			# Cell visual bounds: offset * size - half_size to offset * size + half_size
			var c_center = Vector2(offset) * cell_size
			var c_min = c_center - Vector2(cell_size / 2, cell_size / 2)
			var rect = Rect2(c_min, Vector2(cell_size, cell_size))
			boundary_rects.append(rect)
			
		var params = entry["params"]
		
		# Define callback for param updates
		var on_param_change = func(key, value):
			print("Param changed: %s = %s" % [key, value])
			entry["params"][key] = value
			# Update Visual
			if entry.has("visual") and is_instance_valid(entry["visual"]):
				entry["visual"].update_state(entry["params"])
		
		# TRACK SELECTION FOR DELETE
		selected_entity_pos = main_pos
		selected_trace_idx = -1
		
		# USE main_world_pos (rune anchor) instead of grid_pos (clicked cell)
		properties_panel.setup(rune, main_world_pos, camera, boundary_rects, params, on_param_change)
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
				# For trace, simple box around the point
				var t_box = Rect2(-10, -10, 20, 20)
				properties_panel.setup(t["data"], closest, camera, [t_box])
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
	var cells = rune.get_occupied_cells()
	for offset in cells:
		var occupied_pos = grid_pos + offset
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
