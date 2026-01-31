class_name EditorToolbar
extends PanelContainer

signal tool_changed(tool_name: String)
signal trace_width_changed(width_idx: int)
signal request_start_sim
signal request_stop_sim

@onready var width_option: OptionButton = $HBoxContainer/RouteOptions/WidthOption

func _ready() -> void:
	# Width Options
	width_option.clear()
	width_option.add_item("Small (1)", 0)
	width_option.add_item("Medium (4)", 1)
	width_option.add_item("Large (12)", 2)
	width_option.select(0)


func _on_select_pressed() -> void:
	emit_signal("tool_changed", "SELECT")

func _on_place_pressed() -> void:
	emit_signal("tool_changed", "PLACE")

func _on_route_pressed() -> void:
	emit_signal("tool_changed", "ROUTE")

func _on_width_option_item_selected(index: int) -> void:
	emit_signal("trace_width_changed", index)

func _on_play_pressed() -> void:
	print("Toolbar: Play pressed")
	emit_signal("request_start_sim")

func _on_stop_pressed() -> void:
	print("Toolbar: Stop pressed")
	emit_signal("request_stop_sim")
