class_name GridSystem
extends RefCounted

## Handles coordinate conversion, grid visualization, and snapping logic.

var cell_size: Vector2 = Vector2(40, 40)
var grid_offset: Vector2 = Vector2(0, 0)
var grid_width_cells: int = 20
var grid_height_cells: int = 15

# Color settings
var grid_color_major: Color = Color(0.6, 0.6, 0.6, 0.5) # Lighter grey
var grid_color_minor: Color = Color(0.4, 0.4, 0.4, 0.3)


func _init(_cell_size: Vector2, _width: int, _height: int, _offset: Vector2 = Vector2.ZERO):
	cell_size = _cell_size
	grid_width_cells = _width
	grid_height_cells = _height
	grid_offset = _offset

## Converts world position (screen pixels) to logic Grid position (x, y).
func world_to_grid(world_pos: Vector2) -> Vector2i:
	var local = world_pos - grid_offset
	var gx = floori(local.x / cell_size.x)
	var gy = floori(local.y / cell_size.y)
	return Vector2i(gx, gy)

## Converts Grid position to center World position.
func grid_to_world(grid_pos: Vector2i) -> Vector2:
	return grid_offset + (Vector2(grid_pos) * cell_size) + (cell_size / 2.0)

## Snaps a raw world position to the nearest grid cell center.
func snap_to_grid(world_pos: Vector2) -> Vector2:
	var gp = world_to_grid(world_pos)
	return grid_to_world(gp)

## Checks if a grid position is within bounds.
func is_valid_pos(grid_pos: Vector2i) -> bool:
	return grid_pos.x >= 0 and grid_pos.x < grid_width_cells and \
		   grid_pos.y >= 0 and grid_pos.y < grid_height_cells

## Draws the grid on the given CanvasItem (usually the EngravingTable).
func draw_grid(canvas: Node2D) -> void:
	var total_w = grid_width_cells * cell_size.x
	var total_h = grid_height_cells * cell_size.y
	
	# Draw background area (Dark Slate) - Infinite-ish
	var huge_rect = Rect2(Vector2(-10000, -10000), Vector2(20000, 20000))
	canvas.draw_rect(huge_rect, Color(0.1, 0.1, 0.12, 1.0), true)
	
	# Draw Playable Area Highlight (slightly lighter?)
	var play_area = Rect2(grid_offset, Vector2(total_w, total_h))
	canvas.draw_rect(play_area, Color(0.12, 0.12, 0.15, 1.0), true)
	
	# Vertical Lines

	for x in range(grid_width_cells + 1):
		var start = grid_offset + Vector2(x * cell_size.x, 0)
		var end = start + Vector2(0, total_h)
		canvas.draw_line(start, end, grid_color_major, 1.0)
		
	# Horizontal Lines
	for y in range(grid_height_cells + 1):
		var start = grid_offset + Vector2(0, y * cell_size.y)
		var end = start + Vector2(total_w, 0)
		canvas.draw_line(start, end, grid_color_major, 1.0)
