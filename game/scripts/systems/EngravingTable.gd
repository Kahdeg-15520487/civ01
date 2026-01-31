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
@export var grid_size: int = 40
@export var grid_width: int = 20
@export var grid_height: int = 14 # Reduced to fit 648p screen

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
	_setup_ui()

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

func _setup_ui() -> void:
	# Create a CanvasLayer to hold UI so it stays above the game world
	var ui_layer = CanvasLayer.new()
	add_child(ui_layer)
	
	# Instantiate and anchor the palette
	var palette_scene = preload("res://scenes/ui/RunePalette.tscn")
	rune_palette_ui = palette_scene.instantiate()
	ui_layer.add_child(rune_palette_ui)
	
	# Connect signal
	rune_palette_ui.rune_selected.connect(_on_rune_selected)

	
	# Select first one by default if available
	if rune_palette_ui.available_runes.size() > 0:
		selected_rune_type = rune_palette_ui.available_runes[0]

func _on_rune_selected(rune: Rune) -> void:
	print("Selected rune for placement: ", rune.display_name)
	selected_rune_type = rune

func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		queue_redraw()
		if is_dragging_trace and current_trace_visual:
			var mouse_pos = get_local_mouse_position()
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
				_handle_click(event.position)
				
		elif event.button_index == MOUSE_BUTTON_RIGHT:
			# Right click to draw traces
			if event.pressed:
				_start_trace(event.position)
			else:
				_end_trace(event.position)

func _start_trace(screen_pos: Vector2) -> void:
	var grid_pos = grid_system.world_to_grid(screen_pos)
	if not grid_system.is_valid_pos(grid_pos): return
	
	is_dragging_trace = true
	drag_start_grid = grid_pos
	
	# Create Data
	var trace_data = QiTrace.new()
	trace_data.width = 10.0 # Default width for now
	trace_data.start_point = grid_system.grid_to_world(grid_pos)
	trace_data.end_point = trace_data.start_point # Initially same
	
	# Create Visual
	current_trace_visual = QiTraceVisualScene.instantiate()
	trace_layer.add_child(current_trace_visual)
	current_trace_visual.setup(trace_data)

	
	print("Started drawing trace from: ", grid_pos)

func _end_trace(screen_pos: Vector2) -> void:
	if not is_dragging_trace: return
	
	var grid_pos = grid_system.world_to_grid(screen_pos)
	print("Ended trace at: ", grid_pos)
	
	# TODO: Validate trace (collision, length)
	# For now, just leave it there
	
	is_dragging_trace = false
	current_trace_visual = null


func _handle_click(screen_pos: Vector2) -> void:
	var grid_pos = grid_system.world_to_grid(screen_pos)
	if grid_system.is_valid_pos(grid_pos):
		print("Clicked on cell: ", grid_pos)
		
		# Place the selected rune
		if selected_rune_type:
			place_rune(selected_rune_type, grid_pos)
		else:
			print("No rune selected!")


func _draw() -> void:
	if grid_system:
		grid_system.draw_grid(self)
		
		# Draw Cursor Highlight
		var mouse_pos = get_local_mouse_position()
		var grid_pos = grid_system.world_to_grid(mouse_pos)
		
		if grid_system.is_valid_pos(grid_pos):
			var snap_pos = grid_system.grid_to_world(grid_pos)
			# Draw a rect centered on snap_pos
			var rect_size = grid_system.cell_size
			var rect = Rect2(snap_pos - rect_size/2.0, rect_size)
			draw_rect(rect, Color(1, 1, 0, 0.3), true) # Yellow highlight
			draw_rect(rect, Color(1, 1, 0, 0.8), false, 2.0) # Border



const RuneVisualScene = preload("res://scenes/systems/RuneVisual.tscn")
const QiTraceVisualScene = preload("res://scenes/systems/QiTraceVisual.tscn")

# State
enum ToolMode { PLACE_RUNE, DRAW_TRACE }
var current_tool: ToolMode = ToolMode.PLACE_RUNE
var is_dragging_trace: bool = false
var current_trace_visual: QiTraceVisual = null
var drag_start_grid: Vector2i


## Called when user drags a rune onto the board
func place_rune(rune_resource: Rune, grid_position: Vector2i) -> void:
	if current_board_state.has(grid_position):
		print("Cell occupied!")
		return
		
	print("Placing rune: %s at %s" % [rune_resource.display_name, grid_position])
	
	var visual = RuneVisualScene.instantiate()
	rune_layer.add_child(visual)
	visual.setup(rune_resource)
	visual.position = grid_system.grid_to_world(grid_position)

	
	current_board_state[grid_position] = {
		"rune": rune_resource,
		"visual": visual
	}


## Starts the Chi flow simulation
func start_simulation() -> void:
	is_simulating = true
	emit_signal("simulation_started")
	_run_tick()

func stop_simulation() -> void:
	is_simulating = false
	emit_signal("simulation_stopped")

func _run_tick() -> void:
	if not is_simulating: return
	
	print("Simulation Tick")
	# Propagate Qi through traces and Runes
	
	# Schedule next tick
	if is_simulating:
		get_tree().create_timer(0.5).timeout.connect(_run_tick)
