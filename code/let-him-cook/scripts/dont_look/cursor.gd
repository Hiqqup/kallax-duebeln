extends TextureRect


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	#pivot_offset=  Vector2(-35, 0)
	pass


func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			AnimationUtil.bounce_tween(self);
	
	if event is InputEventMouseMotion:
		var camera2d = get_viewport().get_camera_2d();
		var mouse_pos
		if camera2d:
			mouse_pos = camera2d.get_global_mouse_position();
		else:
			mouse_pos = get_viewport().get_mouse_position()
		position = mouse_pos - pivot_offset ;
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:

	rotation += delta

	
	
