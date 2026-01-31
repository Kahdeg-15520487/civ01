class_name QiTrace
extends RefCounted

## Represents a connection between two points on the Artifact Board.

var start_point: Vector2
var end_point: Vector2
var width: float = 5.0
var qi_type: String = "neutral" 

## Calculates the required spacing from this trace to another based on their widths.
## Formula: Minimum Distance = (WidthA + WidthB) / 2
static func get_min_spacing(trace_a_width: float, trace_b_width: float) -> float:
	return (trace_a_width + trace_b_width) / 2.0

## Checks if two traces are too close.
static func check_proximity(trace_a: QiTrace, trace_b: QiTrace) -> bool:
	# Simplified check: assumes parallel segments for now or point-line distance.
	# In a full implementation, this would use geometry collision.
	var min_dist = get_min_spacing(trace_a.width, trace_b.width)
	# TODO: Implement actual geometric distance check
	return true
