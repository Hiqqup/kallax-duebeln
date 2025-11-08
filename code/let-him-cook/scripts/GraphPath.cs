using Godot;

public partial class GraphPath : Node2D
{
	[Export] 
	public I_GraphNode ChildNode { get; set; } 
	public I_GraphNode ParentNode { get; set; } 
	
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
	}
}
