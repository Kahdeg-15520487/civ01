class_name PropertiesPanel
extends PanelContainer

@onready var name_label: Label = $VBoxContainer/NameLabel
@onready var type_label: Label = $VBoxContainer/TypeLabel
@onready var desc_label: RichTextLabel = $VBoxContainer/DescLabel
@onready var delete_btn: Button = $VBoxContainer/DeleteButton

signal delete_requested

func setup(rune: Rune) -> void:
	name_label.text = rune.display_name
	type_label.text = "Type: %s" % Rune.RuneType.keys()[rune.type]
	desc_label.text = rune.description
	visible = true

func _on_delete_button_pressed() -> void:
	emit_signal("delete_requested")
	visible = false

func clear() -> void:
	visible = false
