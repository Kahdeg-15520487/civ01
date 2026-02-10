extends PanelContainer

signal primitive_selected(primitive: ArtifactPrimitive)

# Container reference (set up programmatically)
var container: VBoxContainer = null
var scroll_container: ScrollContainer = null

# Categorized Primitive paths
var primitive_paths: Dictionary = {
	"Basic Solids": [
		"res://resources/primitives/cube.tres",
		"res://resources/primitives/sphere.tres",
	],
	"Cylindrical": [
		"res://resources/primitives/cylinder.tres",
		"res://resources/primitives/cone.tres",
	],
	"Advanced": [
		"res://resources/primitives/torus.tres",
	]
}

var primitive_categories: Dictionary = {}

func _ready() -> void:
	# Create UI structure programmatically
	_create_base_ui()
	_load_primitives()
	_populate_palette()

func _create_base_ui() -> void:
	# Create margin container for padding
	var margin = MarginContainer.new()
	margin.add_theme_constant_override("margin_left", 10)
	margin.add_theme_constant_override("margin_top", 10)
	margin.add_theme_constant_override("margin_right", 10)
	margin.add_theme_constant_override("margin_bottom", 10)
	add_child(margin)

	# Create scroll container
	scroll_container = ScrollContainer.new()
	scroll_container.size_flags_horizontal = Control.SIZE_SHRINK_END | Control.SIZE_FILL
	scroll_container.size_flags_vertical = Control.SIZE_SHRINK_END | Control.SIZE_FILL
	scroll_container.horizontal_scroll_mode = ScrollContainer.SCROLL_MODE_DISABLED
	margin.add_child(scroll_container)

	# Create container for primitives
	container = VBoxContainer.new()
	container.name = "PrimitiveList"
	container.size_flags_horizontal = Control.SIZE_SHRINK_END | Control.SIZE_FILL
	container.size_flags_vertical = Control.SIZE_SHRINK_END | Control.SIZE_FILL
	container.add_theme_constant_override("separation", 10)
	scroll_container.add_child(container)

func _load_primitives() -> void:
	primitive_categories.clear()
	for cat in primitive_paths:
		var loaded_list = []
		for path in primitive_paths[cat]:
			if ResourceLoader.exists(path):
				var res = ResourceLoader.load(path, "", ResourceLoader.CACHE_MODE_REPLACE)
				if res is ArtifactPrimitive:
					loaded_list.append(res)
				else:
					push_error("Path is not an ArtifactPrimitive: " + path)
			else:
				push_error("Primitive Resource missing: " + path)

		if loaded_list.size() > 0:
			primitive_categories[cat] = loaded_list

func _populate_palette() -> void:
	print("PrimitivePalette: Populating...")
	if not container:
		push_error("PrimitivePalette: Container is null! Cannot populate.")
		return

	# Clear existing
	for child in container.get_children():
		child.queue_free()

	print("PrimitivePalette: Categories found: ", primitive_categories.keys())

	for category in primitive_categories:
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

		for primitive in primitive_categories[category]:
			var btn = Button.new()
			btn.text = primitive.display_name
			btn.tooltip_text = primitive.description
			btn.custom_minimum_size = Vector2(80, 40)
			btn.pressed.connect(func(): _on_primitive_button_pressed(primitive))
			grid.add_child(btn)

func _on_primitive_button_pressed(primitive: ArtifactPrimitive) -> void:
	print("Palette selected: ", primitive.display_name)
	emit_signal("primitive_selected", primitive)
