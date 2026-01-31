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

func update_endpoint(new_end: Vector2) -> void:
	if line_2d.points.size() > 1:
		line_2d.set_point_position(1, new_end)
