class_name QiTraceVisual
extends Node2D

@onready var line_2d: Line2D = $Line2D

var trace_data: QiTrace

func setup(data: QiTrace) -> void:
	trace_data = data
	line_2d.width = data.width
	line_2d.points = [data.start_point, data.end_point]
	
	# Future: Apply color based on Qi Type
	line_2d.default_color = Color(0.4, 0.8, 0.9, 0.8) # Cyan energy

## Updates the end point of a simple 2-point line
func update_endpoint(new_end: Vector2) -> void:
	if line_2d.points.size() >= 2:
		line_2d.set_point_position(line_2d.points.size() - 1, new_end)

## Set the full path of the trace (for multi-segment lines)
func update_path(new_points: PackedVector2Array) -> void:
	line_2d.points = new_points

