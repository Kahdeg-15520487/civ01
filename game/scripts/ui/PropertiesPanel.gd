class_name PropertiesPanel
extends PanelContainer

@onready var name_label: Label = $VBoxContainer/NameLabel
@onready var type_label: Label = $VBoxContainer/TypeLabel
@onready var desc_label: RichTextLabel = $VBoxContainer/DescLabel
@onready var delete_btn: Button = $VBoxContainer/DeleteButton

signal delete_requested

func setup(entity: Variant) -> void:
	if entity is Rune:
		name_label.text = entity.display_name
		type_label.text = "Type: %s" % Rune.RuneType.keys()[entity.type]
		desc_label.text = entity.description
	elif entity is QiTrace:
		name_label.text = "Qi Trace"
		type_label.text = "Width: %s" % entity.width
		desc_label.text = "Transfers Qi between nodes."
		
	visible = true


func _on_delete_button_pressed() -> void:
	emit_signal("delete_requested")
	visible = false

func clear() -> void:
	visible = false
