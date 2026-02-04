class_name RunePalette
extends PanelContainer

signal rune_selected(rune: Rune)

@onready var container = null

# Categorized Rune List
var rune_categories: Dictionary = {
	"Sources": [
		preload("res://resources/runes/sources/source_socket.tres"),
		preload("res://resources/runes/sources/source_array.tres")
	],
	"Operations": [
		preload("res://resources/runes/operations/op_amplifier.tres"),
		preload("res://resources/runes/operations/op_combiner.tres"),
		preload("res://resources/runes/operations/op_splitter.tres"),
		preload("res://resources/runes/operations/op_attenuator.tres"),
		preload("res://resources/runes/operations/op_transmuter.tres"),
		preload("res://resources/runes/operations/op_dampener.tres"),
		preload("res://resources/runes/tests/test_l_shape.tres")
	],
	"Control": [
		preload("res://resources/runes/control/control_threshold.tres"),
		preload("res://resources/runes/control/control_yinyang.tres"),
		preload("res://resources/runes/control/control_filter.tres")
	],
	"Containers": [
		preload("res://resources/runes/containers/container_vessel.tres"),
		preload("res://resources/runes/containers/container_pool.tres")
	],
	"Stones": [
		preload("res://resources/runes/items/item_stone_fire.tres"),
		preload("res://resources/runes/items/item_stone_water.tres"),
		preload("res://resources/runes/items/item_stone_wood.tres"),
		preload("res://resources/runes/items/item_stone_earth.tres"),
		preload("res://resources/runes/items/item_stone_metal.tres")
	],
	"Sinks": [
		preload("res://resources/runes/sinks/sink_emitter.tres"),
		preload("res://resources/runes/sinks/sink_void.tres"),
		preload("res://resources/runes/sinks/sink_heatsink.tres"),
		preload("res://resources/runes/sinks/sink_grounding.tres")
	]
}

func _ready() -> void:
	# Debugging node path issue and finding container manually
	var margin = get_node_or_null("MarginContainer")
	if not margin:
		push_error("RunePalette: MarginContainer not found!")
		print_tree_pretty()
		return
		
	var scroll = margin.get_node_or_null("ScrollContainer")
	if not scroll:
		push_error("RunePalette: ScrollContainer not found under MarginContainer!")
		margin.print_tree_pretty()
		# Fallback for old structure?
		container = margin.get_node_or_null("VBoxContainer")
	else:
		container = scroll.get_node_or_null("VBoxContainer")
		
	if not container:
		push_error("RunePalette: VBoxContainer not found!")
		print_tree_pretty()
		return

	_populate_palette()

func _populate_palette() -> void:
	print("RunePalette: Populating...")
	if not container:
		push_error("RunePalette: Container is null! Cannot populate.")
		return
		
	# Clear existing
	for child in container.get_children():
		child.queue_free()
		
	print("RunePalette: Categories found: ", rune_categories.keys())
		
	for category in rune_categories:
		print("Adding category: ", category)
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
