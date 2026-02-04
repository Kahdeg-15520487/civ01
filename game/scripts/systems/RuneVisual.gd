class_name RuneVisual
extends Node2D

var rune_data: Rune
var grid_cell_size: float = 20.0 # Default fallback
var label: Label
var port_labels: Array[Label] = []

func setup(_rune: Rune, _cell_size: float = 20.0) -> void:
	rune_data = _rune
	grid_cell_size = _cell_size
	
	# 1. Calculate Bounding Box of the shape for centering
	var min_bound = Vector2(9999, 9999)
	var max_bound = Vector2(-9999, -9999)
	var cells = rune_data.get_occupied_cells()
	
	for cell in cells:
		min_bound.x = min(min_bound.x, cell.x)
		min_bound.y = min(min_bound.y, cell.y)
		max_bound.x = max(max_bound.x, cell.x)
		max_bound.y = max(max_bound.y, cell.y)
		
	# Convert grid bounds to pixel bounds (relative to pivot center)
	var bound_center_grid = (min_bound + max_bound) / 2.0
	var pixel_center = bound_center_grid * grid_cell_size
	
	# 2. Configure Main Label
	if not label:
		label = Label.new()
		add_child(label)
		label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		label.z_index = 1
	
	# Use a large font size and scale down for SDF-like crispness
	# High-DPI approach: Render huge, scale tiny.
	var base_font_size = 128
	var text_scale = 0.125 # 128 * 0.125 = 16 effective
	
	label.text = rune_data.display_name.left(3)
	label.add_theme_font_size_override("font_size", base_font_size)
	label.add_theme_color_override("font_outline_color", Color.BLACK)
	label.add_theme_constant_override("outline_size", 32) # Scale outline with font size
	
	label.scale = Vector2(text_scale, text_scale)
	
	label.reset_size()
	var l_size = label.get_minimum_size()
	label.position = pixel_center - ((l_size * text_scale) / 2.0)
	
	# 3. Configure Port Labels
	_update_port_labels()
	
	queue_redraw()

func _update_port_labels() -> void:
	# Clear existing port labels (or pool them, but recreation is cheap enough here)
	for l in port_labels:
		l.queue_free()
	port_labels.clear()
	
	if not rune_data.io_definition: return
	
	var base_font_size = 96
	var text_scale = 0.08 # 96 * 0.08 = ~7.6 effective
	
	for port_name in rune_data.io_definition:
		var grid_rel_pos = rune_data.io_definition[port_name]
		var port_pixel_pos = Vector2(grid_rel_pos) * grid_cell_size
		
		var l = Label.new()
		add_child(l)
		l.text = port_name.left(1).capitalize()
		l.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		l.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		l.z_index = 2
		
		l.add_theme_font_size_override("font_size", base_font_size)
		# Add outline for readability
		l.add_theme_color_override("font_outline_color", Color.BLACK)
		l.add_theme_constant_override("outline_size", 24) # Scaled outline
		
		l.scale = Vector2(text_scale, text_scale)
		
		l.reset_size()
		var size = l.get_minimum_size()
		# Position: Port pos is center of port. 
		# We want text slightly below/offset? Or centered on port?
		# Original code: port_pixel_pos + Vector2(-5, 12)
		# Let's try centering on the port but slightly offset Y to not cover the dot.
		var offset = Vector2(0, 4)
		l.position = port_pixel_pos - ((size * text_scale) / 2.0) + offset
		
		port_labels.append(l)

func _draw() -> void:
	if rune_data:
		# Calculate actual size in pixels
		var cell_size = grid_cell_size
		
		# Draw Cells
		var cells = rune_data.get_occupied_cells()
		var sockets = rune_data.socket_pattern
		
		for offset in cells:
			var cell_pos = Vector2(offset) * cell_size
			var rect = Rect2(cell_pos - Vector2(cell_size / 2, cell_size / 2), Vector2(cell_size, cell_size))
			
			if sockets.has(offset):
				# Draw "Hollow" Socket style
				draw_rect(rect, Color(0.1, 0.1, 0.2), true) # Dark background (Hole)
				draw_rect(rect, Color(0.4, 0.6, 1.0), false, 2.0) # Light Border
			else:
				# Draw "Solid" Body style
				draw_rect(rect, Color(0.2, 0.4, 0.8), true) # Blueish Body
				draw_rect(rect, Color.WHITE, false, 1.0) # Border
			
	# Draw Ports
		if rune_data.io_definition:
			for port_name in rune_data.io_definition:
				var grid_rel_pos = rune_data.io_definition[port_name]
				var port_pixel_pos = (Vector2(grid_rel_pos) * cell_size)
				
				# Draw Port Marker
				var is_input = not ("out" in port_name.to_lower() or "pass" in port_name.to_lower() or "block" in port_name.to_lower() or "excess" in port_name.to_lower())
				var color = Color(0.0, 1.0, 0.0) if is_input else Color(1.0, 0.4, 0.0) # Green In, Orange Out
				
				draw_circle(port_pixel_pos, 4.0, color)
				# Text handled by Labels now
		
		# Draw Slotted Stone (if any)
		if current_params.has("stone_type"):
			var type = current_params["stone_type"]
			var color = _get_element_color(type)
			
			# Target position: (0,0) relative to center is usually the explicit "Hole" for Array/Socket
			# For Socket (2x2 U-shape), (0,0) is also the empty spot in our def.
			var stone_pos = Vector2(0, 0)
			
			draw_circle(stone_pos, cell_size * 0.3, color)
			draw_circle(stone_pos, cell_size * 0.2, Color.WHITE.lerp(color, 0.5)) # Shine

var current_params: Dictionary = {}

func update_state(params: Dictionary) -> void:
	current_params = params
	queue_redraw()

func _get_element_color(element: String) -> Color:
	match element:
		"Fire": return Color(1, 0.2, 0.2)
		"Water": return Color(0.2, 0.2, 1)
		"Wood": return Color(0.2, 0.8, 0.2)
		"Earth": return Color(0.8, 0.7, 0.2)
		"Metal": return Color(0.9, 0.9, 0.9)
		_: return Color.GRAY
