using Godot;
using System;

public partial class CameraController : Camera2D
{

	[Export] private float _moveSpeed = 5.0f;
	[Export] private float _zoomSpeed = 10.0f;
	
	private Vector2 _zoomTarget;
	
	// Unity start
	public override void _Ready()
	{
		_zoomTarget = Zoom;
	}

	// Unity update
	public override void _Process(double delta)
	{
		Zoomy(delta);
		SimplePan(delta);
		ClickAndDrag();
	}

	private void Zoomy(double delta)
	{
		if (Input.IsActionJustPressed("camera_zoom_in"))
		{
			GD.Print("Zoom in");
			_zoomTarget *= 1.1f;
		}

		if (Input.IsActionJustPressed("camera_zoom_out"))
		{
			GD.Print("Zoom out");
			_zoomTarget *= 0.9f;
		}
		
		Zoom = Zoom.Slerp(_zoomTarget, (float)(_zoomSpeed * delta));
	}

	private void SimplePan(double delta)
	{
		Vector2 moveDirection =  Vector2.Zero;
		
		if (Input.IsActionPressed("camera_move_up"))
		{
			GD.Print("Move up");
			moveDirection.Y -= 1.0f;
		}
		
		if (Input.IsActionPressed("camera_move_down"))
		{
			GD.Print("Move down");
			moveDirection.Y += 1.0f;
		}
		
		if (Input.IsActionPressed("camera_move_right"))
		{
			GD.Print("Move right");
			moveDirection.X += 1.0f;
		}
		
		if (Input.IsActionPressed("camera_move_left"))
		{
			GD.Print("Move left");
			moveDirection.X -= 1.0f;
			
		}

		if (moveDirection != Vector2.Zero)
		{
			moveDirection = moveDirection.Normalized();
			Position += moveDirection * _moveSpeed * (float)delta;
		}
		
	}

	private void ClickAndDrag()
	{
		
	}
}
