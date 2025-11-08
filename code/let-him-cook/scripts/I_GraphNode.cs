using Godot;
using System;
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
	public ProductionResource Output { get; set; }

	public delegate void InputSatisfiedHandler();
	public event InputSatisfiedHandler OnInputSatisfied;
	
	[Export]
	public bool MouseOver = false;
	public bool Selected = false;
	public bool FollowMouse = false;
	private Vector2 _mouseOffset;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize the input inventory from the Input array
		_inputInventory = new SystemDictionary();
		
		ResetInputInventory();
		
		if (Paths != null)
		{
			foreach (var graphPaths in Paths)
			{
				if (graphPaths == null) continue;
				graphPaths.ParentNode = this;
			}
		}
		
		if (Recource_Input == null || Recource_Input.Count == 0)
		{
			//GD.Print("This node is a root node " + this.Name);
			ProduceOutput();
		}

		OnInputSatisfied += () => { GD.Print("Input satisfied"); };
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
	}

	public void ProduceOutput()
	{
		if (Output != ProductionResource.None && Paths != null)
		{
			foreach (var graphPath in Paths)
			{
				graphPath?.Transport(Output);
			}
		}
	}

	/**
	 * Checks if all items in the inventory are 0. If they are the input is satisfied
	 */
	private bool IsInputSatisfied()
	{
		return _inputInventory.Values.All(count => count == 0);
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
		if(!FollowMouse)
		{
			MoveAndCollide(Vector2.Zero);
		}
	}
}
