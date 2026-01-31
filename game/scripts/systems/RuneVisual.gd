class_name RuneVisual
extends Node2D

var rune_data: Rune

func setup(_rune: Rune) -> void:
	rune_data = _rune
	queue_redraw()

func _draw() -> void:
	if rune_data:
		# Draw background box
		var rect = Rect2(-20, -20, 40, 40) # Assumes 40x40 cell, centered
		draw_rect(rect, Color(0.2, 0.4, 0.8), true) # Blueish
		
		# Draw Border
		draw_rect(rect, Color.WHITE, false, 2.0)
		
		# Draw Name (Placeholder for Sprite)
		var font = ThemeDB.fallback_font
		draw_string_outline(font, Vector2(-15, 5), rune_data.display_name.left(3), HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER, -1, 16, 1, Color.BLACK)
		draw_string(font, Vector2(-15, 5), rune_data.display_name.left(3), HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER, -1, 16)
