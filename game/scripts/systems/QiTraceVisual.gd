class_name QiTraceVisual
extends Node2D

@onready var line_2d: Line2D = $Line2D
@onready var invalid_line: Line2D # For red invalid segment

var trace_data: QiTrace
var crossing_point: Vector2 = Vector2.INF # INF means no crossing
var has_crossing: bool = false

func setup(data: QiTrace) -> void:
	trace_data = data
	line_2d.width = data.width
	line_2d.points = [data.start_point, data.end_point]
	
	# Create invalid segment line
	if not invalid_line:
		invalid_line = Line2D.new()
		add_child(invalid_line)
		invalid_line.width = data.width
		invalid_line.default_color = Color(1, 0.2, 0.2, 0.9) # Red
	
	# Future: Apply color based on Qi Type
	line_2d.default_color = Color(0.4, 0.8, 0.9, 0.8) # Cyan energy

## Updates the end point of a simple 2-point line
func update_endpoint(new_end: Vector2) -> void:
	if line_2d.points.size() >= 2:
		line_2d.set_point_position(line_2d.points.size() - 1, new_end)

## Set the full path of the trace (for multi-segment lines)
## Optionally accepts crossing info
func update_path(new_points: PackedVector2Array, cross_point: Vector2 = Vector2.INF) -> void:
	crossing_point = cross_point
	has_crossing = cross_point != Vector2.INF
	
	if has_crossing:
		# Split path: valid (cyan) up to crossing, invalid (red) from crossing onwards
		var valid_points: PackedVector2Array = []
		var invalid_points: PackedVector2Array = []
		var passed_crossing = false
		
		for i in range(new_points.size()):
			var p = new_points[i]
			if not passed_crossing:
				valid_points.append(p)
				# Check if the crossing point lies between this point and the next
				if i < new_points.size() - 1:
					var next_p = new_points[i + 1]
					if _point_on_segment(cross_point, p, next_p):
						valid_points.append(cross_point)
						invalid_points.append(cross_point)
						passed_crossing = true
			else:
				invalid_points.append(p)
		
		line_2d.points = valid_points
		invalid_line.points = invalid_points
		invalid_line.visible = true
	else:
		line_2d.points = new_points
		invalid_line.points = []
		invalid_line.visible = false
	
	queue_redraw()

func _point_on_segment(point: Vector2, seg_start: Vector2, seg_end: Vector2) -> bool:
	# Check if point is approximately on the segment
	var d1 = point.distance_to(seg_start)
	var d2 = point.distance_to(seg_end)
	var seg_len = seg_start.distance_to(seg_end)
	return abs((d1 + d2) - seg_len) < 5.0 # Tolerance
