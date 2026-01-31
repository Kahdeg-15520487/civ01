class_name RunePalette
extends PanelContainer

signal rune_selected(rune: Rune)

@onready var container = $MarginContainer/VBoxContainer

# Categorized Rune List
var rune_categories: Dictionary = {
	"Sources": [
		preload("res://resources/runes/sources/source_fire.tres")
	],
	"Operations": [
		preload("res://resources/runes/operations/op_amplifier.tres"),
		preload("res://resources/runes/operations/op_combiner.tres"),
		preload("res://resources/runes/operations/op_splitter.tres"),
		preload("res://resources/runes/operations/op_attenuator.tres")
	],
	"Control": [
		preload("res://resources/runes/control/control_threshold.tres")
	],
	"Containers": [
		preload("res://resources/runes/containers/container_vessel.tres")
	],
	"Sinks": [
		preload("res://resources/runes/sinks/sink_emitter.tres"),
		preload("res://resources/runes/sinks/sink_void.tres")
	]
}

func _ready() -> void:
	_populate_palette()

func _populate_palette() -> void:
	# Clear existing (if any)
	for child in container.get_children():
		child.queue_free()
		
	for category in rune_categories:
		# Header
		var header = Label.new()
		header.text = category
		header.add_theme_font_size_override("font_size", 16)
		header.add_theme_color_override("font_color", Color(0.7, 0.9, 1.0))
		container.add_child(header)
		
		# Grid for buttons
		var grid = GridContainer.new()
		grid.columns = 2
		container.add_child(grid)
		
		for rune in rune_categories[category]:
			var btn = Button.new()
			btn.text = rune.display_name
			btn.tooltip_text = rune.description
			btn.custom_minimum_size = Vector2(80, 40)
			# Bind the rune to the callback
			btn.pressed.connect(func(): _on_rune_button_pressed(rune))
			grid.add_child(btn)

func _on_rune_button_pressed(rune: Rune) -> void:
	print("Palette selected: ", rune.display_name)
	emit_signal("rune_selected", rune)
