class_name Rune
extends Resource

## Base class for all programmable Runes in the game.

enum RuneType {
	SOURCE,    # Generates Qi
	PROCESSOR, # Modifies Qi (Logic gates, Elemental conversion)
	STORAGE,   # Holds Qi
	OUTPUT     # Uses Qi (Artifact effect)
}

@export_group("Identity")
@export var id: String = "rune_base"
@export var display_name: String = "Base Rune"
@export_multiline var description: String = ""
@export var icon: Texture2D

@export_group("Mechanics")
@export var type: RuneType = RuneType.PROCESSOR
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
