using Godot;
using System;

public partial class CameraController : Camera2D
{

	[Export] private float _moveSpeed = 750.0f;
	[Export] private float _zoomSpeed = 10.0f;
	
	private Vector2 _zoomTarget;
	
	private bool _isDragging = false;
	private Vector2 _dragStartMousePos = Vector2.Zero;
	private Vector2 _dragStartCameraPos = Vector2.Zero;
	
	// Unity start
	public override void _Ready()
	{
		_zoomTarget = Zoom;
		UpdateMovementSpeed();
		GetTree().Root.SizeChanged += OnWindowSizeChanged;
	}

	private void UpdateMovementSpeed()
	{
		_moveSpeed = DisplayServer.ScreenGetSize().X / 2.0f;
	}
	
	private void OnWindowSizeChanged()
	{
		UpdateMovementSpeed();
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
			//GD.Print("Zoom in");
			_zoomTarget *= 1.1f;
		}

		if (Input.IsActionJustPressed("camera_zoom_out"))
		{
			//GD.Print("Zoom out");
			_zoomTarget *= 0.9f;
		}
		
		Zoom = Zoom.Slerp(_zoomTarget, (float)(_zoomSpeed * delta));
	}

	private void SimplePan(double delta)
	{
		Vector2 moveDirection =  Vector2.Zero;
		
		if (Input.IsActionPressed("camera_move_up"))
		{
			//GD.Print("Move up");
			moveDirection.Y -= 1.0f;
		}
		
		if (Input.IsActionPressed("camera_move_down"))
		{
			//GD.Print("Move down");
			moveDirection.Y += 1.0f;
		}
		
		if (Input.IsActionPressed("camera_move_right"))
		{
			//GD.Print("Move right");
			moveDirection.X += 1.0f;
		}
		
		if (Input.IsActionPressed("camera_move_left"))
		{
			//GD.Print("Move left");
			moveDirection.X -= 1.0f;
			
		}

		if (moveDirection != Vector2.Zero)
		{
			moveDirection = moveDirection.Normalized();
			Position += moveDirection * _moveSpeed * (float)delta * (1/Zoom.X);
		}
		
	}

	private void ClickAndDrag()
	{
		
		
		if (!_isDragging && Input.IsActionJustPressed("camera_pan"))
		{
			_isDragging = true;
			_dragStartMousePos = GetViewport().GetMousePosition();
			_dragStartCameraPos = Position;
		}

		if (_isDragging && Input.IsActionJustReleased("camera_pan"))
		{
			_isDragging = false;
		}

		if (_isDragging)
		{
			Vector2 moveVector = GetViewport().GetMousePosition() - _dragStartMousePos;
			Position = _dragStartCameraPos - moveVector * (1/Zoom.X);
		}
	}
}
