using Godot;

public partial class GraphPath : Node2D
{
    private I_GraphNode _childNode;
    [Export]
    private I_GraphNode ChildNode { 
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
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Length = 3;
        Speed = 1;
    }

    public void Transport(ProductionResource input)
    {
        TransportedItem = input;
        Active = true;
        Progress = 0;
        //GD.Print(this.Name + ": Started Transport of " + TransportedItem);
    }

    private void UpdateLine()
    {
        _line.SetPoints(new []{ParentNode.Position, ChildNode.Position});
        var pathVector = ChildNode.Position - ParentNode.Position;
        _collisionShape.Position = ParentNode.Position + pathVector / 2;
        _collisionShape.Scale = new(pathVector.Length() - 160, 1f);
        _collisionShape.Rotation = pathVector.Angle();
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
}