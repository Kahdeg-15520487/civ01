class_name PropertiesPanel
extends PanelContainer

@onready var name_label: Label = $VBoxContainer/NameLabel
@onready var type_label: Label = $VBoxContainer/TypeLabel
@onready var desc_label: RichTextLabel = $VBoxContainer/DescLabel
@onready var delete_btn: Button = $VBoxContainer/DeleteButton

signal delete_requested

var target_world_pos: Vector2
var target_boundaries: Array[Rect2] = [] # List of AABBs relative to target_world_pos (unzoomed)
var camera_ref: Camera2D

var is_dragging: bool = false
var drag_offset: Vector2

# Setup now takes a list of boundaries
func setup(entity: Variant, world_pos: Vector2, camera: Camera2D, boundaries: Array[Rect2] = [Rect2(-20, -20, 40, 40)]) -> void:
	target_world_pos = world_pos
	target_boundaries = boundaries
	camera_ref = camera
	
	if entity is Rune:
		name_label.text = entity.display_name
		type_label.text = "Type: %s" % Rune.RuneType.keys()[entity.type]
		desc_label.text = entity.description
	elif entity is QiTrace:
		name_label.text = "Qi Trace"
		type_label.text = "Width: %s" % entity.width
		desc_label.text = "Transfers Qi between nodes."
		
	# Initial Positioning: Near the target
	if camera_ref:
		# Force resize to fit content immediately
		reset_size()
		
		# Position panel near the top-right corner of the Union AABB
		# Calculate Union AABB
		var union_aabb = Rect2()
		if not target_boundaries.is_empty():
			union_aabb = target_boundaries[0]
			for b in target_boundaries:
				union_aabb = union_aabb.merge(b)
		
		var screen_center = get_viewport_rect().size / 2.0
		var screen_anchor = (target_world_pos - camera_ref.position) * camera_ref.zoom + screen_center
		var zoomed_aabb_center = (union_aabb.position + union_aabb.size / 2.0) * camera_ref.zoom
		
		# Default pos: 60px right of anchor
		position = screen_anchor + zoomed_aabb_center + Vector2(60, -50)
		
	visible = true

func _gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT:
			if event.pressed:
				is_dragging = true
				drag_offset = position - get_global_mouse_position()
			else:
				is_dragging = false

func _process(_delta: float) -> void:
	if visible:
		if is_dragging:
			position = get_global_mouse_position() + drag_offset
		queue_redraw()

func _draw() -> void:
	if visible and camera_ref:
		var screen_center = get_viewport_rect().size / 2.0
		var target_screen_anchor = (target_world_pos - camera_ref.position) * camera_ref.zoom + screen_center
		
		var center_local = size / 2.0
		var target_local_anchor = target_screen_anchor - position
		
		# Calculate Union Center for aiming
		var union_aabb = Rect2()
		if not target_boundaries.is_empty():
			union_aabb = target_boundaries[0]
			for b in target_boundaries:
				union_aabb = union_aabb.merge(b)
				
		var zoomed_center_offset = (union_aabb.position + union_aabb.size / 2.0) * camera_ref.zoom
		var target_visual_center = target_local_anchor + zoomed_center_offset
		
		var dir_vec = target_visual_center - center_local
		var half_size = size / 2.0
		
		# 1. Panel Clipping
		var dx = abs(dir_vec.x)
		var dy = abs(dir_vec.y)
		var t_start = 1.0
		
		if dx > 0.001:
			t_start = min(t_start, half_size.x / dx)
		if dy > 0.001:
			t_start = min(t_start, half_size.y / dy)
			
		var start_point = center_local + dir_vec * t_start
		
		# 2. Target Clipping (Multi-Rect)
		# We cast ray from Panel Center (center_local) towards Target Center.
		# We want the FIRST intersection with ANY rect.
		var min_t_end = 1.0 # Cap at center
		var found_intersection = false
		
		# Ray props
		var ray_origin = center_local
		var ray_dir = dir_vec # To target
		
		for b in target_boundaries:
			# Box in local space
			var box_min = target_local_anchor + b.position * camera_ref.zoom
			var box_max = box_min + b.size * camera_ref.zoom
			
			var t_enter = -99999.0
			var t_exit = 99999.0
			
			# Slab check X
			if abs(ray_dir.x) > 0.001:
				var t1 = (box_min.x - ray_origin.x) / ray_dir.x
				var t2 = (box_max.x - ray_origin.x) / ray_dir.x
				t_enter = max(t_enter, min(t1, t2))
				t_exit = min(t_exit, max(t1, t2))
			
			# Slab check Y
			if abs(ray_dir.y) > 0.001:
				var t1 = (box_min.y - ray_origin.y) / ray_dir.y
				var t2 = (box_max.y - ray_origin.y) / ray_dir.y
				t_enter = max(t_enter, min(t1, t2))
				t_exit = min(t_exit, max(t1, t2))
				
			if t_enter <= t_exit and t_exit >= 0:
				# Intersection exists
				# We want the entry point (smallest positive t)
				if t_enter >= 0:
					if t_enter < min_t_end:
						min_t_end = t_enter
						found_intersection = true
				elif t_exit >= 0:
					# Inside box? Use exit? No, if we are inside, we shouldn't draw line through.
					# But here we assume Panel is outside.
					pass
		
		var end_point = center_local + dir_vec * min_t_end
		
		if start_point.distance_to(end_point) > 5.0 and found_intersection:
			draw_line(start_point, end_point, Color.WHITE, 2.0)
			draw_circle(end_point, 4.0, Color.WHITE)
		elif not found_intersection:
			 # Fallback if no intersection (e.g. inside all boxes or logic error): draw to center
			draw_line(start_point, target_visual_center, Color.WHITE, 2.0) # Debug fallback


func _on_delete_button_pressed() -> void:
	emit_signal("delete_requested")
	visible = false

func clear() -> void:
	visible = false
