using Godot;
using System;

public partial class PathPreview : Node2D
{
	[Export]
	private Line2D _line;

	[Export] private Sprite2D _dot;
	
	private Vector2 _endPosition;
	
	private float _progress;
	

	public Vector2 EndPosition
	{
		get => _endPosition;
		set
		{
			_endPosition = value;
			UpdateLine();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	private void UpdateLine()
	{
		_line.SetPoints(new []{new(0, 0), -(GetParent<Node2D>().Position - EndPosition)});
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var direction = (EndPosition - GetParent<Node2D>().Position).Normalized();
		var length = (EndPosition - GetParent<Node2D>().Position).Length();
		_dot.Position = direction * _progress;
		_progress += (float)delta * Constants.PreviewDotSpeed;
		if (_progress >= length)
		{
			_progress = 80;
		}
	}
}
