using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot.Collections;
using SystemDictionary = System.Collections.Generic.Dictionary<ProductionResource, int>;

public partial class I_GraphNode : CharacterBody2D
{
	[Export]
	public Array<GraphPath> Paths { get; set; }
	
	[Export]
	public Array<ResourceAmount> Recource_Input { get; set; } = new Array<ResourceAmount>();

	// Internal dictionary for easy access - maps resource to required amount
	private SystemDictionary _inputInventory;
	[Export] 
	public Array<ResourceAmount> Output { get; set; } = new Array<ResourceAmount>();

	public delegate void InputSatisfiedHandler();
	public event InputSatisfiedHandler OnInputSatisfied;

	private NodeType _nodeType = NodeType.None;
	
	private ResourceAmount _producedResourceBuffer = new ResourceAmount();
	private Queue<GraphPath> _pathQueue = new Queue<GraphPath>();
	/**
	 * Should only be used by producer
	 */
	private Timer _resourceProductionTimer = new Timer();

	[Export]
	public bool MouseOver = false;
	public bool Selected = false;
	public bool FollowMouse = false;
	private Vector2 _mouseOffset;
	private CollisionShape2D  _collisionShape2D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		// Initialize the input inventory from the Input array
		_inputInventory = new SystemDictionary();
		
		DetectNodeType();
		ResetInputInventory();
		
		if (Recource_Input == null || Recource_Input.Count == 0)
		{
			GD.Print("This node is a root node " + this.Name);
			ProduceOutput();
		}
		
		if (Paths != null)
		{
			foreach (var graphPaths in Paths)
			{
				if (graphPaths == null) continue;
				graphPaths.ParentNode = this;
				
				PathFinished(graphPaths);
			}
		}
		
		if (_nodeType == NodeType.Producer)
		{
			AddChild(_resourceProductionTimer);
			_resourceProductionTimer.OneShot = false;
			_resourceProductionTimer.Start(1);
			_resourceProductionTimer.Timeout += ProduceOutput;
		}

		OnInputSatisfied += () => { GD.Print(this.Name + ": Input satisfied"); };
	}

	public void PathFinished(GraphPath path)
	{
		_pathQueue.Enqueue(path);

		if (_producedResourceBuffer.Amount > 0)
		{
			TryConsumeResource();
		}
	}

	private void TryConsumeResource()
	{
		if (_pathQueue.Count == 0)
		{
			return;
		}
		
		var path = _pathQueue.Dequeue();
		path.Transport(_producedResourceBuffer.Resource);
		_producedResourceBuffer.Amount--;
		//GD.Print(path.Name + " started transporting " + _producedResourceBuffer.Resource.ToString());
	}

	private void DetectNodeType()
	{
		bool hasInput = Recource_Input is { Count: > 0 };
		bool hasOutput = Output != null && Output.Count > 0 && Output[0].Resource != ProductionResource.None;

		if (hasInput && hasOutput)
		{
			//GD.Print("Node " + this.Name + " is a Factory");
			_nodeType = NodeType.Factory;
			return;
		}

		if (hasInput)
		{
			//GD.Print("Node \"" + this.Name + "\" is a Consumer");
			_nodeType = NodeType.Consumer;
			return;
		}

		if (hasOutput)
		{
			//GD.Print("Node \"" + this.Name + "\" is a Producer");
			_nodeType = NodeType.Producer;
			return;
		}
	}

	private void ResetInputInventory()
	{
		if (Recource_Input == null || Recource_Input.Count <= 0) return;
		
		foreach (var resourceAmount in Recource_Input)
		{
			if (resourceAmount?.Resource == null || resourceAmount.Resource == ProductionResource.None || resourceAmount.Amount <= 0) continue;
			
			_inputInventory[resourceAmount.Resource] = resourceAmount.Amount;
			//GD.Print("Input: " + resourceAmount.Resource + " " + resourceAmount.Amount + "x");
		}

		//GD.Print("Reset inventory for node " + this.Name);
	}

	public void ProduceOutput()
	{
		if (Output == null || Output.Count == 0 || Output[0].Resource == ProductionResource.None) return;
		
		_producedResourceBuffer.Resource = Output[0].Resource;
		_producedResourceBuffer.Amount = Math.Clamp(_producedResourceBuffer.Amount + Output[0].Amount, 0, Output[0].Amount * 2);
		//GD.Print($"Produced resource: {_producedResourceBuffer.Resource}, stored amount: {_producedResourceBuffer.Amount}");
		
		if (_producedResourceBuffer.Amount > 0)
		{
			TryConsumeResource();
		}
	}

	/**
	 * Checks if all items in the inventory are 0. If they are the input is satisfied
	 */
	private bool IsInputSatisfied()
	{
		return _inputInventory.Values.All(count => count <= 0);
	}
	
	public void ReceiveInput(ProductionResource input)
	{
		//GD.Print(input.ToString() + " received");
		if (_inputInventory.ContainsKey(input))
		{
			_inputInventory[input]--;
			if (IsInputSatisfied())
			{
				OnInputSatisfied?.Invoke();
				
				ResetInputInventory();
				ProduceOutput();
			}
		}
	}

	public void _on_area_2d_mouse_entered()
	{
		MouseOver = true;
	}

	public void _on_area_2d_mouse_exited()
	{
		MouseOver = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && MouseOver)
		{
			// mouse left
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				Selected = !Selected;
				if (mouseEvent.Pressed)
				{
					_mouseOffset = Position - GetViewport().GetMousePosition();
					FollowMouse =  true;
				}
				else
				{
					FollowMouse = false;
				}
				
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (FollowMouse)
		{
			Position = GetViewport().GetMousePosition() + _mouseOffset;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (FollowMouse)
		{
			_collisionShape2D.Disabled = true;
		}
		else
		{
			_collisionShape2D.Disabled = false;
			MoveAndCollide(Vector2.Zero);
			
		}
	}
}
