class_name Rune
extends Resource

## Base class for all programmable Runes in the game.

enum RuneType {
	SOURCE, # Generates Qi
	PROCESSOR, # Modifies Qi (Logic gates, Elemental conversion)
	STORAGE, # Holds Qi
	OUTPUT # Uses Qi (Artifact effect)
}

@export_group("Identity")
@export var id: String = "rune_base"
@export var display_name: String = "Base Rune"
@export_multiline var description: String = ""
@export var icon: Texture2D

@export_group("Mechanics")
@export var type: RuneType = RuneType.PROCESSOR
@export_group("Simulation")
@export var simulation_classes: String = "Civ.Emulator.SpiritStoneSource" # C# Class Name
@export var inputs: Array[Vector2i] = [] # Local grid coords for inputs
@export var outputs: Array[Vector2i] = [Vector2i(0, 0)] # Local grid coords for outputs
@export var size_in_cells: Vector2i = Vector2i(4, 4) # Default 4x4 (equivalent to old 1x1)

## Custom Shape Definition.
## If empty, uses size_in_cells to create a rectangle.
## If not empty, defines the specific cells this rune occupies relative to the pivot (0,0).
@export var shape_pattern: Array[Vector2i] = []

## Returns the list of occupied cells relative to the pivot (0,0).
func get_occupied_cells() -> Array[Vector2i]:
	if not shape_pattern.is_empty():
		return shape_pattern
	
	# Fallback to rectangle
	# We center the 4x4 rect around (0,0) or stick to Top-Left?
	# Previous logic implied Center-ish placement.
	# Let's standardize: If no pattern, generating a centered rect is hard without half-coords.
	# For simplicity in 2D grids, Top-Left (0,0) to (w,h) is easiest, but "Main Position" usually implies center.
	# Let's assume Top-Left relative to pivot for the rectangle for now, 
	# OR simpler: Pivot is center.
	# 4x4 -> (-2,-2) to (1,1).
	var cells: Array[Vector2i] = []
	var top_left = - (size_in_cells / 2)
	for x in range(size_in_cells.x):
		for y in range(size_in_cells.y):
			cells.append(top_left + Vector2i(x, y))
	return cells
@export var qi_cost: int = 1
@export var bandwidth_in: int = 10
@export var bandwidth_out: int = 10


## Dictionary defining input/output slots.
## Format: { "slot_name": Vector2i(grid_x, grid_y) } relative to center.
@export var io_definition: Dictionary = {
	"in": Vector2i(-1, 0),
	"out": Vector2i(1, 0)
}

func _init() -> void:
	pass

## Virtual function to execute the rune's logic.
## Returns the modified Qi packet or null if failed.
func process_qi(input_qi: Dictionary) -> Dictionary:
	return input_qi
