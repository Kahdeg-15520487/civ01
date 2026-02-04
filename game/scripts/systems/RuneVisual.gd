class_name RuneVisual
extends Node2D

var rune_data: Rune
var grid_cell_size: float = 20.0 # Default fallback
var label: Label

func setup(_rune: Rune, _cell_size: float = 20.0) -> void:
	rune_data = _rune
	grid_cell_size = _cell_size
	
	# Create Label if not exists
	if not label:
		label = Label.new()
		add_child(label)
		label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		label.add_theme_font_size_override("font_size", 12)
		label.add_theme_color_override("font_outline_color", Color.BLACK)
		label.add_theme_constant_override("outline_size", 4)
		label.z_index = 1 # On top of rects
	
	label.text = rune_data.display_name.left(3)
	# Center label roughly
	label.position = Vector2(-20, -10)
	label.custom_minimum_size = Vector2(40, 20)
	
	queue_redraw()

func _draw() -> void:
	if rune_data:
		# Calculate actual size in pixels
		var cell_size = grid_cell_size
		
		# Draw Cells
		var cells = rune_data.get_occupied_cells()
		for offset in cells:
			var cell_pos = Vector2(offset) * cell_size
			var rect = Rect2(cell_pos - Vector2(cell_size / 2, cell_size / 2), Vector2(cell_size, cell_size))
			
			draw_rect(rect, Color(0.2, 0.4, 0.8), true) # Blueish Body
			draw_rect(rect, Color.WHITE, false, 1.0) # Border
			
		# Draw Ports
		var font = ThemeDB.fallback_font
		if rune_data.io_definition:
			for port_name in rune_data.io_definition:
				var grid_rel_pos = rune_data.io_definition[port_name]
				var port_pixel_pos = (Vector2(grid_rel_pos) * cell_size) # Relative to (0,0) which is center of rune?
				# Wait, logic check:
				# Rune Center is at (0,0) locally.
				# io_definition coords like (1,0) are relative to the grid center of the rune.
				# A 4x4 rune has center at cross of cells?
				# Let's assume io_definition is relative to the "Main Position" which is the center-ish.
				# So (1,0) means 1 cell right of center.
				
				# Draw Port Marker
				var is_input = not ("out" in port_name.to_lower() or "pass" in port_name.to_lower() or "block" in port_name.to_lower() or "excess" in port_name.to_lower())
				var color = Color(0.0, 1.0, 0.0) if is_input else Color(1.0, 0.4, 0.0) # Green In, Orange Out
				
				draw_circle(port_pixel_pos, 4.0, color)
				
				# Optional: Draw port Label small
				draw_string(font, port_pixel_pos + Vector2(-5, 12), port_name.left(1).capitalize(), HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER, -1, 8, Color.WHITE)
