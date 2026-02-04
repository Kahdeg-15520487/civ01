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
var current_params: Dictionary = {}
var on_param_change: Callable

var is_dragging: bool = false
var drag_offset: Vector2

@onready var container = $VBoxContainer

# Setup now takes params and callback
func setup(entity: Variant, world_pos: Vector2, camera: Camera2D, boundaries: Array[Rect2] = [Rect2(-20, -20, 40, 40)], params: Dictionary = {}, callback: Callable = Callable()) -> void:
	target_world_pos = world_pos
	target_boundaries = boundaries
	camera_ref = camera
	current_params = params
	on_param_change = callback
	
	# Clear previous dynamic UI (anything after DescLabel)
	# Assuming children order: Name, Type, Desc, Delete... 
	# Actually, easier to have a dedicated container.
	# For now, let's remove children named "ConfigContainer" if any
	var existing = container.get_node_or_null("ConfigContainer")
	if existing:
		existing.free() # IMPORTANT: Use free() not queue_free() to avoid duplicates
	
	if entity is Rune:
		name_label.text = entity.display_name
		type_label.text = "Type: %s" % Rune.RuneType.keys()[entity.type]
		desc_label.text = entity.description
		
		# Create Config UI if needed
		_build_config_ui(entity)
		
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


func _build_config_ui(rune: Rune) -> void:
	var config_box = VBoxContainer.new()
	config_box.name = "ConfigContainer"
	container.add_child(config_box)
	container.move_child(config_box, 3) # After Desc
	
	if rune.id == "source_socket":
		_add_single_stone_selector(config_box)
	elif not rune.socket_pattern.is_empty():
		# Dynamic multi-socket support
		_add_multi_stone_selector(config_box, rune.socket_pattern.size())

func _add_single_stone_selector(parent: Node) -> void:
	var lbl = Label.new()
	lbl.text = "Insert Spirit Stone:"
	parent.add_child(lbl)
	
	var opt = _create_element_dropdown()
	# Load current
	if current_params.has("stone_type"):
		_select_element_in_dropdown(opt, current_params["stone_type"])
	
	opt.item_selected.connect(func(idx):
		var val = _get_element_from_dropdown(idx)
		if on_param_change.is_valid():
			on_param_change.call("stone_type", val)
	)
	parent.add_child(opt)

func _add_multi_stone_selector(parent: Node, count: int) -> void:
	var lbl = Label.new()
	lbl.text = "Socket Array (%d Slots):" % count
	parent.add_child(lbl)
	
	# Ensure params has array
	if not current_params.has("element_slots") or not current_params["element_slots"] is Array:
		# Initialize with explicit sizing if needed, but array is flexible
		var defaults = []
		defaults.resize(count)
		defaults.fill("None")
		if on_param_change.is_valid(): # Init immediately? Or wait for input.
			# Let's not auto-init to save bandwidth, treat missing as None
			pass
			
	var current_slots = current_params.get("element_slots", [])
	if current_slots.size() < count:
		current_slots.resize(count)
		current_slots.fill("None") # Logic hole: fill only new
	
	for i in range(count):
		var h_box = HBoxContainer.new()
		parent.add_child(h_box)
		
		var s_lbl = Label.new()
		s_lbl.text = "#%d" % (i + 1)
		h_box.add_child(s_lbl)
		
		var opt = _create_element_dropdown()
		# Get value safely
		var val = "None"
		if i < current_slots.size(): val = current_slots[i]
		_select_element_in_dropdown(opt, val)
		
		opt.item_selected.connect(func(idx):
			var new_val = _get_element_from_dropdown(idx)
			# We need to copy the array, modify, and set it back
			var slots = current_params.get("element_slots", []).duplicate()
			if slots.size() < count:
				slots.resize(count)
				# Fill defaults if resizing
				for j in range(slots.size()):
					if slots[j] == null: slots[j] = "None"
			
			slots[i] = new_val
			
			if on_param_change.is_valid():
				on_param_change.call("element_slots", slots)
		)
		h_box.add_child(opt)

func _create_element_dropdown() -> OptionButton:
	var opt = OptionButton.new()
	opt.add_item("Empty", 0)
	opt.set_item_disabled(0, true) # "Select Element" or "Empty"? Let's allow removing stones.
	# Actually, usually 0 is prompt. Let's make index 0 "None"/Remove.
	opt.set_item_text(0, "Empty")
	opt.set_item_disabled(0, false)
	
	opt.add_item("Fire", 1)
	opt.add_item("Water", 2)
	opt.add_item("Wood", 3)
	opt.add_item("Earth", 4)
	opt.add_item("Metal", 5)
	return opt

func _select_element_in_dropdown(opt: OptionButton, element: String) -> void:
	var types = ["None", "Fire", "Water", "Wood", "Earth", "Metal"]
	# Map "Empty" to "None"
	if element == "Empty": element = "None"
	var idx = types.find(element)
	if idx != -1: opt.selected = idx
	else: opt.selected = 0

func _get_element_from_dropdown(idx: int) -> String:
	var elements = ["None", "Fire", "Water", "Wood", "Earth", "Metal"]
	if idx >= 0 and idx < elements.size():
		return elements[idx]
	return "None"


func _on_delete_button_pressed() -> void:
	emit_signal("delete_requested")
	visible = false

func clear() -> void:
	visible = false
