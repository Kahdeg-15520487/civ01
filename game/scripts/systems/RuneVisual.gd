class_name RuneVisual
extends Node2D

var rune_data: Rune

func setup(_rune: Rune) -> void:
	rune_data = _rune
	queue_redraw()

func _draw() -> void:
	if rune_data:
		# Calculate actual size in pixels (assuming 10px grid)
		var cell_size = 10.0
		var pixel_size = Vector2(rune_data.size_in_cells) * cell_size
		var half_size = pixel_size / 2.0
		
		# Draw background box centered
		var rect = Rect2(-half_size, pixel_size)
		draw_rect(rect, Color(0.2, 0.4, 0.8), true) # Blueish
		
		# Draw Border
		draw_rect(rect, Color.WHITE, false, 2.0)
		
		# Draw Name (Placeholder for Sprite)
		var font = ThemeDB.fallback_font
		draw_string_outline(font, Vector2(-15, 5), rune_data.display_name.left(3), HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER, -1, 16, 1, Color.BLACK)
		draw_string(font, Vector2(-15, 5), rune_data.display_name.left(3), HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER, -1, 16)

		# Draw Ports
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
