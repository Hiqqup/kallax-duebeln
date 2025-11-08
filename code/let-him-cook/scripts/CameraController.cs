using Godot;
using System;

public partial class CameraController : Camera2D
{

	private Vector2 _zoomTarget;
	[Export] private float _zoomSpeed = 10.0f;
	
	// Unity start
	public override void _Ready()
	{
		_zoomTarget = Zoom;
	}

	// Unity update
	public override void _Process(double delta)
	{
		Zoomy(delta);
		SimplePan();
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

	private void SimplePan()
	{
		if (Input.IsActionJustPressed("camera_up"))
		{
			
		}
	}

	private void ClickAndDrag()
	{
		
	}
}
