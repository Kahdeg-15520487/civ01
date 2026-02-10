extends PanelContainer

signal transform_changed(prim_id: String, property: String, value: Variant)
signal parameter_changed(prim_id: String, parameter: String, value: Variant)
signal csg_changed(prim_id: String, operation: int, parent_id: String)
signal delete_requested(prim_id: String)

# UI references (set up programmatically)
var name_label: Label = null
var type_label: Label = null
var desc_label: RichTextLabel = null
var delete_btn: Button = null
var main_container: VBoxContainer = null

var current_primitive: ArtifactPrimitive = null
var current_visual: MeshInstance3D = null
var current_camera: Camera3D = null
var config_container: VBoxContainer = null

func _ready() -> void:
	# Create base UI structure
	_create_base_ui()
	delete_btn.pressed.connect(_on_delete_button_pressed)

func _create_base_ui() -> void:
	# Create main container
	main_container = VBoxContainer.new()
	main_container.name = "VBoxContainer"
	main_container.add_theme_constant_override("separation", 8)
	add_child(main_container)

	# Name label
	name_label = Label.new()
	name_label.name = "NameLabel"
	name_label.text = "Primitive Name"
	main_container.add_child(name_label)

	# Separator
	var sep = HSeparator.new()
	main_container.add_child(sep)

	# Type label
	type_label = Label.new()
	type_label.name = "TypeLabel"
	type_label.text = "Type: Unknown"
	main_container.add_child(type_label)

	# Description label
	desc_label = RichTextLabel.new()
	desc_label.name = "DescLabel"
	desc_label.text = "Description"
	desc_label.fit_content = true
	main_container.add_child(desc_label)

	# Delete button
	delete_btn = Button.new()
	delete_btn.name = "DeleteButton"
	delete_btn.text = "Delete Primitive"
	main_container.add_child(delete_btn)

func setup(primitive: ArtifactPrimitive, visual: MeshInstance3D, camera: Camera3D) -> void:
	current_primitive = primitive
	current_visual = visual
	current_camera = camera

	# Update basic info
	name_label.text = primitive.display_name
	type_label.text = "Type: %s | CSG: %s" % [primitive.get_type_name(), primitive.get_csg_op_name()]
	desc_label.text = primitive.description

	# Remove old config container
	if config_container and is_instance_valid(config_container):
		config_container.queue_free()

	# Create new config UI
	config_container = VBoxContainer.new()
	config_container.name = "ConfigContainer"
	main_container.add_child(config_container)
	main_container.move_child(config_container, 4)  # After DescLabel (index 3), before DeleteButton

	# Build the config UI
	_build_transform_ui()
	_build_parameter_ui()
	_build_csg_ui()

	visible = true

func _build_transform_ui() -> void:
	# Position section
	_add_section_header("Position (Grid)")
	_add_vector3_input("position", current_primitive.position, func(x, y, z):
		transform_changed.emit(current_primitive.id, "position", Vector3i(x, y, z))
	)

	# Rotation section
	_add_section_header("Rotation (Degrees)")
	_add_vector3_input("rotation", current_primitive.rotation, func(x, y, z):
		transform_changed.emit(current_primitive.id, "rotation", Vector3(x, y, z))
	)

	# Scale section
	_add_section_header("Scale")
	_add_vector3_input("scale", current_primitive.scale, func(x, y, z):
		transform_changed.emit(current_primitive.id, "scale", Vector3(x, y, z))
	)

func _build_parameter_ui() -> void:
	_add_section_header("Primitive Parameters")

	match current_primitive.type:
		ArtifactPrimitive.PrimitiveType.CUBE:
			_add_vector3_input("Size", current_primitive.size, func(x, y, z):
				parameter_changed.emit(current_primitive.id, "size", Vector3(x, y, z))
			)
		ArtifactPrimitive.PrimitiveType.SPHERE:
			_add_float_input("Radius", current_primitive.radius, func(v):
				parameter_changed.emit(current_primitive.id, "radius", v)
			)
		ArtifactPrimitive.PrimitiveType.CYLINDER:
			_add_float_input("Radius", current_primitive.radius, func(v):
				parameter_changed.emit(current_primitive.id, "radius", v)
			)
			_add_float_input("Height", current_primitive.height, func(v):
				parameter_changed.emit(current_primitive.id, "height", v)
			)
		ArtifactPrimitive.PrimitiveType.CONE:
			_add_float_input("Radius", current_primitive.radius, func(v):
				parameter_changed.emit(current_primitive.id, "radius", v)
			)
			_add_float_input("Height", current_primitive.height, func(v):
				parameter_changed.emit(current_primitive.id, "height", v)
			)
		ArtifactPrimitive.PrimitiveType.TORUS:
			_add_float_input("Radius", current_primitive.radius, func(v):
				parameter_changed.emit(current_primitive.id, "radius", v)
			)
			_add_float_input("Tube Radius", current_primitive.tube_radius, func(v):
				parameter_changed.emit(current_primitive.id, "tube_radius", v)
			)

func _build_csg_ui() -> void:
	_add_section_header("CSG Operation")

	# CSG operation dropdown
	var opt = OptionButton.new()
	opt.add_item("Union (Add)", 0)
	opt.add_item("Difference (Subtract)", 1)
	opt.add_item("Intersection", 2)
	opt.selected = current_primitive.csg_op
	config_container.add_child(opt)

	opt.item_selected.connect(func(idx):
		csg_changed.emit(current_primitive.id, idx, current_primitive.parent_id)
	)

	# Parent selector (for now just a text entry, could be a dropdown)
	var hbox = HBoxContainer.new()
	var lbl = Label.new()
	lbl.text = "Parent ID:"
	lbl.custom_minimum_size = Vector2(80, 0)
	hbox.add_child(lbl)

	var parent_entry = LineEdit.new()
	parent_entry.text = current_primitive.parent_id
	parent_entry.placeholder_text = "Empty for root"
	hbox.add_child(parent_entry)
	config_container.add_child(hbox)

	parent_entry.text_submitted.connect(func(text):
		csg_changed.emit(current_primitive.id, current_primitive.csg_op, text)
	)

func _add_section_header(text: String) -> void:
	var lbl = Label.new()
	lbl.text = text
	lbl.add_theme_font_size_override("font_size", 14)
	lbl.add_theme_color_override("font_color", Color(0.8, 0.9, 1.0))
	config_container.add_child(lbl)

func _add_vector3_input(label: String, value: Vector3, callback: Callable) -> void:
	var hbox = HBoxContainer.new()

	var lbl = Label.new()
	lbl.text = label
	lbl.custom_minimum_size = Vector2(80, 0)
	hbox.add_child(lbl)

	for i in range(3):
		var spin = SpinBox.new()
		spin.min_value = -100
		spin.max_value = 100
		spin.step = 0.1
		spin.value = value[i]
		spin.custom_minimum_size = Vector2(60, 0)
		hbox.add_child(spin)

		spin.value_changed.connect(func(v):
			var x = hbox.get_child(1).value if i == 0 else value.x
			var y = hbox.get_child(2).value if i == 1 else value.y
			var z = hbox.get_child(3).value if i == 2 else value.z
			callback.call(x, y, z)
		)

	config_container.add_child(hbox)

func _add_float_input(label: String, value: float, callback: Callable) -> void:
	var hbox = HBoxContainer.new()

	var lbl = Label.new()
	lbl.text = label
	lbl.custom_minimum_size = Vector2(80, 0)
	hbox.add_child(lbl)

	var spin = SpinBox.new()
	spin.min_value = 0.1
	spin.max_value = 100
	spin.step = 0.1
	spin.value = value
	hbox.add_child(spin)

	spin.value_changed.connect(func(v):
		callback.call(v)
	)

	config_container.add_child(hbox)

func _on_delete_button_pressed() -> void:
	if current_primitive:
		delete_requested.emit(current_primitive.id)
		visible = false

func clear() -> void:
	visible = false
	current_primitive = null
	current_visual = null
	current_camera = null
