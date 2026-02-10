class_name GridSystem3D extends RefCounted

## 3D Grid system for the Artifact Modeler.
## Handles coordinate conversion between world and grid space in 3D.
## The grid is primarily on the XZ plane (horizontal ground) with Y as up.

var cell_size: Vector3 = Vector3(1, 1, 1)
var grid_extents: Vector3i = Vector3i(20, 20, 20)
var grid_origin: Vector3 = Vector3.ZERO

# Visual settings
var grid_color_major: Color = Color(0.5, 0.5, 0.5, 0.6)
var grid_color_minor: Color = Color(0.3, 0.3, 0.3, 0.3)
var grid_color_axis_x: Color = Color(0.8, 0.2, 0.2, 0.8)  # Red X-axis
var grid_color_axis_z: Color = Color(0.2, 0.2, 0.8, 0.8)  # Blue Z-axis
var grid_color_axis_y: Color = Color(0.2, 0.8, 0.2, 0.8)  # Green Y-axis

func _init(_cell_size: Vector3 = Vector3(1, 1, 1), _extents: Vector3i = Vector3i(20, 20, 20), _origin: Vector3 = Vector3.ZERO):
	cell_size = _cell_size
	grid_extents = _extents
	grid_origin = _origin

## Convert world position to grid position (integer coordinates)
func world_to_grid(world_pos: Vector3) -> Vector3i:
	var local = world_pos - grid_origin
	var gx = floori(local.x / cell_size.x)
	var gy = floori(local.y / cell_size.y)
	var gz = floori(local.z / cell_size.z)
	return Vector3i(gx, gy, gz)

## Convert grid position to world position (center of cell)
func grid_to_world(grid_pos: Vector3i) -> Vector3:
	return grid_origin + (Vector3(grid_pos) * cell_size) + (cell_size / 2.0)

## Snap a world position to the nearest grid cell center
func snap_to_grid(world_pos: Vector3) -> Vector3:
	var gp = world_to_grid(world_pos)
	return grid_to_world(gp)

## Snap to grid on XZ plane only (for placement on ground)
func snap_to_grid_xz(world_pos: Vector3) -> Vector3:
	var gp = world_to_grid(world_pos)
	var snapped = grid_to_world(gp)
	# Preserve Y (height) position
	snapped.y = world_pos.y
	return snapped

## Check if a grid position is within bounds
func is_valid_pos(grid_pos: Vector3i) -> bool:
	return grid_pos.x >= -grid_extents.x / 2 and grid_pos.x < grid_extents.x / 2 and \
		   grid_pos.y >= -grid_extents.y / 2 and grid_pos.y < grid_extents.y / 2 and \
		   grid_pos.z >= -grid_extents.z / 2 and grid_pos.z < grid_extents.z / 2

## Check if position is valid on XZ plane (ignoring Y)
func is_valid_pos_xz(grid_pos: Vector3i) -> bool:
	return grid_pos.x >= -grid_extents.x / 2 and grid_pos.x < grid_extents.x / 2 and \
		   grid_pos.z >= -grid_extents.z / 2 and grid_pos.z < grid_extents.z / 2

## Get the world bounds of the grid
func get_world_bounds() -> AABB:
	var min_pos = grid_origin - (Vector3(grid_extents) * cell_size / 2.0)
	var size = Vector3(grid_extents) * cell_size
	return AABB(min_pos, size)

## Generate grid mesh for visualization
func create_grid_mesh() -> ArrayMesh:
	var mesh = ArrayMesh.new()
	var st = SurfaceTool.new()
	st.begin(Mesh.PRIMITIVE_LINES)

	var half_extents = Vector3(grid_extents) / 2.0

	# Draw grid on XZ plane (Y = 0)
	for x in range(-half_extents.x, half_extents.x + 1):
		var start = Vector3(x, 0, -half_extents.z) * cell_size
		var end = Vector3(x, 0, half_extents.z) * cell_size

		# Highlight axes
		var color = grid_color_major
		if x == 0:
			color = grid_color_axis_x  # X-axis in red

		st.set_color(color)
		st.add_vertex(start)
		st.add_vertex(end)

	for z in range(-half_extents.z, half_extents.z + 1):
		var start = Vector3(-half_extents.x, 0, z) * cell_size
		var end = Vector3(half_extents.x, 0, z) * cell_size

		# Highlight axes
		var color = grid_color_major
		if z == 0:
			color = grid_color_axis_z  # Z-axis in blue

		st.set_color(color)
		st.add_vertex(start)
		st.add_vertex(end)

	# Draw Y-axis line (vertical at origin) - shorter, just for reference
	st.set_color(grid_color_axis_y)
	st.add_vertex(Vector3.ZERO)
	st.add_vertex(Vector3(0, 2.0, 0))  # 2 units tall for reference

	st.generate_normals()
	return st.commit()

## Create a cursor mesh for showing current grid position
func create_cursor_mesh() -> ArrayMesh:
	var mesh = ArrayMesh.new()
	var st = SurfaceTool.new()

	# Create a simple 3D cursor (wireframe box with corner markers)
	st.begin(Mesh.PRIMITIVE_LINES)

	var half_cell = cell_size / 2.0
	var cursor_size = cell_size * 1.1  # Slightly larger than cell

	# Bottom face
	st.add_vertex(Vector3(-cursor_size.x, 0, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, 0, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, 0, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, 0, cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, 0, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, 0, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, 0, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, 0, -cursor_size.z))

	# Vertical lines
	st.add_vertex(Vector3(-cursor_size.x, 0, -cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, cursor_size.y, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, 0, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, cursor_size.y, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, 0, cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, cursor_size.y, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, 0, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, cursor_size.y, cursor_size.z))

	# Top face
	st.add_vertex(Vector3(-cursor_size.x, cursor_size.y, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, cursor_size.y, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, cursor_size.y, -cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, cursor_size.y, cursor_size.z))
	st.add_vertex(Vector3(cursor_size.x, cursor_size.y, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, cursor_size.y, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, cursor_size.y, cursor_size.z))
	st.add_vertex(Vector3(-cursor_size.x, cursor_size.y, -cursor_size.z))

	st.generate_normals()
	return st.commit()

## Raycast to find the grid position from mouse position
## Returns: Vector3i grid position or null if invalid
func raycast_to_grid(camera: Camera3D, mouse_pos: Vector2, plane_y: float = 0.0) -> Vector3i:
	# Create a plane at Y = plane_y (default ground)
	var plane = Plane(Vector3.UP, plane_y)

	# Project mouse position into 3D
	var from = camera.project_ray_origin(mouse_pos)
	var dir = camera.project_ray_normal(mouse_pos)

	# Find intersection with plane
	var intersection = plane.intersects_ray(from, dir)

	if intersection:
		var grid_pos = world_to_grid(intersection)
		if is_valid_pos_xz(grid_pos):
			return grid_pos

	return Vector3i(-99999, -99999, -99999)  # Invalid (sentinel value)

## Calculate the height of a grid position (for multi-cell objects)
func get_cell_height(grid_pos: Vector3i) -> int:
	return 1  # Default: all cells are 1 unit tall

## Get the footprint of an object at a grid position
func get_footprint(grid_pos: Vector3i, size: Vector3i) -> Array[Vector3i]:
	var cells: Array[Vector3i] = []
	var half_size = size / 2

	for x in range(grid_pos.x - half_size.x, grid_pos.x + (size.x + 1) / 2):
		for y in range(grid_pos.y, grid_pos.y + size.y):
			for z in range(grid_pos.z - half_size.z, grid_pos.z + (size.z + 1) / 2):
				cells.append(Vector3i(x, y, z))

	return cells
