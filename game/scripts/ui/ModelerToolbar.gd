class_name ModelerToolbar
extends PanelContainer

signal tool_changed(tool_name: String)
signal compile_pressed

@onready var status_label: Label = $HBoxContainer/StatusLabel

var current_tool: String = "SELECT"

func _ready() -> void:
	_update_tool_buttons()

func _on_select_pressed() -> void:
	emit_signal("tool_changed", "SELECT")
	current_tool = "SELECT"
	_update_tool_buttons()
	_set_status("Select Mode: Click to select primitives")

func _on_place_pressed() -> void:
	emit_signal("tool_changed", "PLACE")
	current_tool = "PLACE"
	_update_tool_buttons()
	_set_status("Place Mode: Select a primitive from the palette, then click to place")

func _on_rotate_pressed() -> void:
	emit_signal("tool_changed", "ROTATE")
	current_tool = "ROTATE"
	_update_tool_buttons()
	_set_status("Rotate Mode: Select a primitive to rotate")

func _on_scale_pressed() -> void:
	emit_signal("tool_changed", "SCALE")
	current_tool = "SCALE"
	_update_tool_buttons()
	_set_status("Scale Mode: Select a primitive to scale")

func _on_compile_pressed() -> void:
	print("Toolbar: Compile pressed")
	emit_signal("compile_pressed")
	_set_status("Compiling artifact...")

func _update_tool_buttons() -> void:
	# Update button states to show active tool
	var select_btn = $HBoxContainer/Tools/BtnSelect
	var place_btn = $HBoxContainer/Tools/BtnPlace
	var rotate_btn = $HBoxContainer/Tools/BtnRotate
	var scale_btn = $HBoxContainer/Tools/BtnScale

	if select_btn:
		select_btn.button_pressed = (current_tool == "SELECT")
	if place_btn:
		place_btn.button_pressed = (current_tool == "PLACE")
	if rotate_btn:
		rotate_btn.button_pressed = (current_tool == "ROTATE")
	if scale_btn:
		scale_btn.button_pressed = (current_tool == "SCALE")

func _set_status(message: String) -> void:
	if status_label:
		status_label.text = message

func set_compiling(is_compiling: bool) -> void:
	var compile_btn = $HBoxContainer/CompileContainer/BtnCompile
	if compile_btn:
		compile_btn.disabled = is_compiling
		compile_btn.text = "Compiling..." if is_compiling else "Compile Artifact"

func set_result(success: bool, message: String) -> void:
	if success:
		_set_status("Success: " + message)
	else:
		_set_status("Error: " + message)
