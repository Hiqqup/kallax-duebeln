using Godot;
using System;
using System.Linq;

public partial class UnitSelector : Control
{
	private bool _selecting;
	private Vector2 _dragStart;
	private Rect2 _selectBox;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				_selecting = true;
				//_dragStart = mouseEvent.Position;
				_dragStart = GetViewport().GetCamera2D().GetGlobalMousePosition();
			}
			else
			{
				_selecting = false;
				if (_dragStart.IsEqualApprox(mouseEvent.Position)) 
					_selectBox = new (mouseEvent.Position, Vector2.Zero);
				GD.Print("select box: ", _selectBox);
				UpdateSelectedUnits();
				QueueRedraw();
			}
		} else if (_selecting && @event is InputEventMouseMotion mouseMotion)
		{
			var dragEnd = GetViewport().GetCamera2D().GetGlobalMousePosition();
			//float xMin = Mathf.Min(_dragStart.X, mouseMotion.Position.X);
			//float yMin = Mathf.Min(_dragStart.Y, mouseMotion.Position.Y);
			//_selectBox = new Rect2(
			//	xMin, 
			//	yMin, 
			//	Math.Max(_dragStart.X, dragEnd.X) - xMin,
			//	Math.Max(_dragStart.Y, dragEnd.Y) - yMin);
			_selectBox = new Rect2();
			_selectBox.Position = _dragStart;
			_selectBox.End = dragEnd;
			
			UpdateSelectedUnits();
			QueueRedraw();
		}
		
	}

	public override void _Draw()
	{
		GD.Print("should draw rect");
		if (!_selecting)
		{
			GD.Print("cant draw rect");
			return;
		}
		DrawRect(_selectBox, new Color("#00ff0066"));
		DrawRect(_selectBox, new Color("#00ff00"), false, 2f);
	}

	private void UpdateSelectedUnits()
	{
		foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selectable_units").Cast<I_GraphNode>())
		{
			if (unit.IsInsideSelectionBox(_selectBox))
				unit.Select();
			else unit.Deselect();
		}
	}
}
