using Godot;
using System;

public partial class PathPreview : Node2D
{
	[Export]
	private Line2D _line;
	
	private Vector2 _endPosition;

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
		
	}
}
