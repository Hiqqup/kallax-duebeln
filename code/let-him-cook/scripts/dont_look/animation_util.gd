extends Node
class_name AnimationUtil
static var tween_dictionary: Dictionary[Node, Tween];

static func check_type_error(node: Node)->bool:
	if not (node is Node2D or node is Control):
		printerr("incorrect type supplied - cant tween scale");
		return true;
	return false;

static func bounce_tween(node: Node):
	if check_type_error(node):
		return; 
	
	var tween =  node.create_tween();
	var start_scale = Vector2.ONE;
	var middle_scale = Vector2.ONE * 0.7;
	(tween.tween_property(node, "scale", middle_scale,0.1)
		.from(start_scale).set_trans(Tween.TRANS_BACK));
	(tween.tween_property(node, "scale", start_scale,0.3)
		.from(middle_scale).set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT));
		
static func scale_up_tween(node: Node):
	if check_type_error(node):
		return; 
	
	var tween =  node.create_tween();
	var start_scale = Vector2.ZERO;
	var middle_scale = Vector2.ONE ;
	(tween.tween_property(node, "scale", middle_scale,1.3)
		.from(start_scale).set_trans(Tween.TRANS_ELASTIC).set_ease(Tween.EASE_OUT));


static func scale_down_tween(node: Node):
	if check_type_error(node):
		return; 
	
	var tween =  node.create_tween();
	var start_scale = Vector2.ONE;
	var middle_scale = Vector2.ZERO ;
	(tween.tween_property(node, "scale", middle_scale,0.3)
		.from(start_scale).set_trans(Tween.TRANS_ELASTIC).set_ease(Tween.EASE_OUT));

static func floor_trap_trigger_tween(node: Node):
	if check_type_error(node):
		return; 
	
	var tween =  node.create_tween();
	var start_scale = Vector2.ZERO;
	var middle_scale = Vector2.UP * 7;
	(tween.tween_property(node, "position", middle_scale,0.1)
		.from(start_scale).set_trans(Tween.TRANS_BACK));
	(tween.tween_property(node, "position", start_scale,0.3)
		.from(middle_scale).set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT));
		

static func hotbar_item_fade_out_tween(node: Node):
	if check_type_error(node):
		return; 
	node.visible = true;
	node.modulate = Color.WHITE;
	if tween_dictionary.has(node) and tween_dictionary[node].is_running():
		tween_dictionary[node].stop()
	await node.get_tree().create_timer(0.5).timeout
	var tween: Tween;
	if tween_dictionary.has(node) and tween_dictionary[node].is_running():
		tween_dictionary[node].stop()
		tween = tween_dictionary[node];
	else:
		tween=  node.create_tween();
		tween_dictionary[node] = tween;
	tween.tween_property(node, "modulate", Color(Color.WHITE, 0 ),1);
	tween.play()


static func fade_out_tween(node: Node):
	if check_type_error(node):
		return; 
	
	var tween =  node.create_tween();
	node.visible = true;
	tween.tween_property(node, "modulate", Color(Color.WHITE, 0 ),1);



static func fade_in_tween(node: Node):
	if check_type_error(node):
		return; 
	
	var tween =  node.create_tween();
	node.visible = true;
	node.modulate = Color(Color.WHITE, 0);
	tween.tween_property(node, "modulate", Color.WHITE,1);
	tween.tween_callback(func(): node.visible = false);
