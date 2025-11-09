using Godot;
using System;
using System.Linq;

public partial class UnitSelector : Node2D
{
    private bool _selecting;
    private Vector2 _dragStart;
    private Rect2 _selectBox = new Rect2();
    private const float MIN_DRAG_DISTANCE = 5f; // Minimum pixels to be considered a drag

    public override void _Ready()
    {
        ZIndex = 100;
    }

    public override void _Input(InputEvent @event)
    {
        // Check if any unit is being dragged - if so, don't do box selection
        if (I_GraphNode.IsAnyUnitBeingDragged()) // You'll need to expose this static method
        {
            return;
        }

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseEvent.Pressed)
            {
                // Check if we clicked on a unit
                var mousePos = GetViewport().GetCamera2D().GetGlobalMousePosition();
                bool clickedOnUnit = false;
                
                foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selectable_units").Cast<I_GraphNode>())
                {
                    if (unit.MouseOver)
                    {
                        clickedOnUnit = true;
                        break;
                    }
                }
                
                // Only start box selection if we didn't click on a unit
                if (!clickedOnUnit)
                {
                    _selecting = true;
                    _dragStart = mousePos;
                    
                    // Deselect all units when starting a new box selection
                    foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selected_units").Cast<I_GraphNode>().ToList())
                    {
                        unit.Deselect();
                    }
                }
            }
            else // Released
            {
                if (_selecting)
                {
                    _selecting = false;
                    
                    // Only update selection if we actually dragged (not just a click)
                    var dragDistance = _dragStart.DistanceTo(GetViewport().GetCamera2D().GetGlobalMousePosition());
                    if (dragDistance > MIN_DRAG_DISTANCE)
                    {
                        UpdateSelectedUnits();
                    }
                    
                    QueueRedraw();
                }
            }
        }
        else if (_selecting && @event is InputEventMouseMotion)
        {
            var dragEnd = GetViewport().GetCamera2D().GetGlobalMousePosition();
            _selectBox.Position = _dragStart;
            _selectBox.End = dragEnd;
            
            // Preview selection while dragging
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
            if (unit.IsInsideSelectionBox(_selectBox))
            {
                unit.Select();
            }
            else
            {
                unit.Deselect();
            }
        }
    }
}