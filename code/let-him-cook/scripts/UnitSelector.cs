using Godot;
using System;
using System.Linq;
using Vector2 = Godot.Vector2;

public partial class UnitSelector : Node2D
{
	private bool _selecting;
	private Vector2 _dragStart;
	private Rect2 _selectBox = new Rect2();

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				_selecting = true;
				_dragStart = GetViewport().GetCamera2D().GetGlobalMousePosition();
			}
			else
			{
				_selecting = false;
				if (_dragStart.IsEqualApprox(mouseEvent.Position))
				{
					_selectBox.Position = mouseEvent.Position;
					_selectBox.Size = Vector2.Zero;
				}
				UpdateSelectedUnits();
				QueueRedraw();
			}
		} else if (_selecting && @event is InputEventMouseMotion mouseMotion)
		{
			var dragEnd = GetViewport().GetCamera2D().GetGlobalMousePosition();
			_selectBox.Position = _dragStart;
			_selectBox.End = dragEnd;
			
			UpdateSelectedUnits();
			QueueRedraw();
		}
		
	}

	public override void _Draw()
	{
		if (!_selecting)
		{
			return;
		}
		DrawRect(_selectBox, new Color("#00ff0066"));
		DrawRect(_selectBox, new Color("#00ff00"), false, 2f);
	}

	private void UpdateSelectedUnits()
	{
		foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selectable_units").Cast<I_GraphNode>())
		{
			if (unit.IsInsideSelectionBox(_selectBox)) unit.Select();
			else unit.Deselect();
		}
	}
}
