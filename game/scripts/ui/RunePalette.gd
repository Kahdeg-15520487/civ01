class_name RunePalette
extends PanelContainer

signal rune_selected(rune: Rune)

@onready var container = $MarginContainer/VBoxContainer

# Sample list of runes (in a real game, this comes from player inventory/unlocks)
var available_runes: Array[Rune] = [
	preload("res://resources/runes/source.tres"),
	preload("res://resources/runes/gate.tres")
]

func _ready() -> void:
	_populate_palette()

func _populate_palette() -> void:
	for rune in available_runes:
		var btn = Button.new()
		btn.text = rune.display_name
		btn.tooltip_text = rune.description
		# Bind the rune to the callback
		btn.pressed.connect(func(): _on_rune_button_pressed(rune))
		container.add_child(btn)

func _on_rune_button_pressed(rune: Rune) -> void:
	print("Palette selected: ", rune.display_name)
	emit_signal("rune_selected", rune)
