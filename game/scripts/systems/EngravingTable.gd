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

func _ready() -> void:
	print("Engraving Table Initialized")
	_setup_grid()
	_setup_ui()

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
		# Update cursor position
		queue_redraw()
	elif event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			_handle_click(event.position)

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

## Called when user drags a rune onto the board
func place_rune(rune_resource: Rune, grid_position: Vector2i) -> void:
	if current_board_state.has(grid_position):
		print("Cell occupied!")
		return
		
	print("Placing rune: %s at %s" % [rune_resource.display_name, grid_position])
	
	var visual = RuneVisualScene.instantiate()
	add_child(visual)
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
