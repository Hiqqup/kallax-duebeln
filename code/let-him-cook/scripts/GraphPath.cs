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
        GD.Print("Started Transport of " + TransportedItem.ToString());
    }

    private void UpdateLine()
    {
        _line.SetPoints(new []{ParentNode.Position, ChildNode.Position});
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
                    ParentNode.ProduceOutput();
                }
            }
        }
        UpdateLine();
    }
}