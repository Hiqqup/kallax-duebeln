using System.Linq;
using Godot;

public partial class GraphPath : Node2D
{
    private I_GraphNode _childNode;
    [Export]
    public I_GraphNode ChildNode { 
        get => _childNode;
        set{
            _childNode = value;
            if (_parentNode != null && _childNode != null)
            {
                UpdateLine();
            }
        } 
    }

    private I_GraphNode _parentNode;

    public I_GraphNode ParentNode
    {
        get => _parentNode;
        set{
            _parentNode = value;
            if (ParentNode != null && ChildNode != null)
            {
                UpdateLine();
            }
        }
    }

    [Export]
    private Line2D _line;
    [Export]
    private CollisionShape2D _collisionShape;
	
    private float Length { get; set; }
    private float Speed { get; set; }
    private float Progress { get; set; }
    private bool Active { get; set; }
    private ProductionResource TransportedItem { get; set; }
    
    private bool _hovered = false;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Length = 3;
        Speed = 200;
    }

    public void Transport(ProductionResource input)
    {
        TransportedItem = input;
        Active = true;
        Progress = 0;
    }

    private void UpdateLine()
    {
        var startPos = ParentNode.Position;
        var endPos = ChildNode.Position;
        var direction = (endPos - startPos).Normalized();
        
        startPos = ParentNode.Position + (direction * Constants.NodeRadius);
        endPos = ChildNode.Position - (direction * Constants.NodeRadius);
        
        _line.SetPoints(new []{startPos, endPos});
        var pathVector = endPos - startPos;
        var relativeCenter = (endPos - startPos) / 2.0f;
        _collisionShape.Position = startPos + relativeCenter;
        _collisionShape.Scale = new(pathVector.Length(), 1f);
        _collisionShape.Rotation = pathVector.Angle();
        
        Length = startPos.DistanceTo(endPos);
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Active)
        {
            Progress += Speed * (float)delta;
            if (Progress >= Length)
            {
                Active = false;
                if (ChildNode != null)
                {
                    ChildNode.ReceiveInput(TransportedItem);
                    TransportedItem = ProductionResource.None;
                    ParentNode.PathFinished(this);
                }
            }
        }
        UpdateLine();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && _hovered)
        {
            // mouse right
            if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                ParentNode.Paths.Remove(ParentNode.Paths.FirstOrDefault(x => x == this));
                QueueFree();
            }
        }
    }

    private void OnMouseEntered()
    {
        _hovered = true;
    }

    private void OnMouseExited()
    {
        _hovered = false;
    }
}