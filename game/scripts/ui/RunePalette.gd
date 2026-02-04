class_name RunePalette
extends PanelContainer

signal rune_selected(rune: Rune)

@onready var container = null

# Categorized Rune Paths (Strings) to prevent preload crashes
var rune_paths: Dictionary = {
	"Sources": [
		"res://resources/runes/sources/source_socket.tres",
		"res://resources/runes/sources/source_array.tres"
	],
	"Operations": [
		"res://resources/runes/operations/op_amplifier.tres",
		"res://resources/runes/operations/op_combiner.tres",
		"res://resources/runes/operations/op_splitter.tres",
		"res://resources/runes/operations/op_attenuator.tres",
		"res://resources/runes/operations/op_transmuter.tres",
		"res://resources/runes/operations/op_dampener.tres",
		"res://resources/runes/tests/test_l_shape.tres"
	],
	"Control": [
		"res://resources/runes/control/control_threshold.tres",
		"res://resources/runes/control/control_yinyang.tres",
		"res://resources/runes/control/control_filter.tres"
	],
	"Containers": [
		"res://resources/runes/containers/container_vessel.tres",
		"res://resources/runes/containers/container_pool.tres"
	],
	"Stones": [
		"res://resources/runes/items/item_stone_fire.tres",
		"res://resources/runes/items/item_stone_water.tres",
		"res://resources/runes/items/item_stone_wood.tres",
		"res://resources/runes/items/item_stone_earth.tres",
		"res://resources/runes/items/item_stone_metal.tres"
	],
	"Sinks": [
		"res://resources/runes/sinks/sink_emitter.tres",
		"res://resources/runes/sinks/sink_void.tres",
		"res://resources/runes/sinks/sink_heatsink.tres",
		"res://resources/runes/sinks/sink_grounding.tres"
	]
}
var rune_categories: Dictionary = {} # Will be populated with Resources

func _ready() -> void:
	# Recursive search for the container to be absolutely safe
	container = find_child("RuneList", true, false)
	
	if not container:
		# Fallback: maybe it's still named VBoxContainer in some cached version?
		container = find_child("VBoxContainer", true, false)
	
	if not container:
		push_error("CRITICAL: RuneList container not found in RunePalette!")
		print("Dumping Scene Tree for Debug:")
		print_tree_pretty()
		return
	
	print("RunePalette: Container found: ", container.name)
	_load_runes()
	_populate_palette()

func _load_runes() -> void:
	rune_categories.clear()
	for cat in rune_paths:
		var loaded_list = []
		for path in rune_paths[cat]:
			if ResourceLoader.exists(path):
				# Force reload to ensure socket_pattern updates are picked up DO NOT CACHE
				var res = ResourceLoader.load(path, "", ResourceLoader.CACHE_MODE_REPLACE)
				if res is Rune:
					loaded_list.append(res)
				else:
					push_error("Path is not a Rune: " + path)
			else:
				push_error("Rune Resource missing: " + path)
		
		# Only add category if it has items
		if loaded_list.size() > 0:
			rune_categories[cat] = loaded_list

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
