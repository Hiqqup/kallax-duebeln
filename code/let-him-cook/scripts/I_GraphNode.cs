using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using global::LetHimCook.scripts.global;
using Godot.Collections;
using SystemDictionary = System.Collections.Generic.Dictionary<ProductionResource, int>;

public partial class I_GraphNode : CharacterBody2D
{
	[Export] public Array<GraphPath> Paths { get; private set; } = [];
	
	[Export]
	//Input
	public Array<ResourceAmount> Recource_Input { get; set; } = new Array<ResourceAmount>(); //max
	
	private SystemDictionary _inputInventory; //buffer
	[Export]
	//Output
	public Array<ResourceAmount> Output { get; set; } = new Array<ResourceAmount>(); //max
	private ResourceAmount _producedResourceBuffer = new ResourceAmount(); //buffer

	// Internal dictionary for easy access - maps resource to required amount
	

	public delegate void InputSatisfiedHandler();
	public event InputSatisfiedHandler OnInputSatisfied;

	public delegate void ConsumerTaskCompletedHandler();
	public event ConsumerTaskCompletedHandler OnConsumerTaskCompleted;

	public NodeType NodeType { get; set; } = NodeType.None;
	
	
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
	private static bool _isConnecting = false;
	private static PathPreview _preview;
	private static I_GraphNode _pathOrigin;
	private static I_GraphNode _lastHovered;
	private CollisionShape2D  _collisionShape2D;
	private Sprite2D _fillSprite2D;
	private Label _statusLabel;

	private readonly float _taskTimeSeconds = 5.0f;
	
	private Timer _questDuration;

	private Camera2D _cam;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		 if (true) // type == producer
		 {
			 _cam = GetViewport().GetCamera2D();
			 // selector.Hide(); if there is a visible selector
		 } 
		 
		 if (GetViewport().GetCamera2D() == null) GD.PrintErr("Camera not found! Please add camera to your scene with a camera manager script attached.");
		 
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		_statusLabel = GetNode<Label>("StatusLabel");
		// Initialize the input inventory from the Input array
		_inputInventory = new SystemDictionary();
		_fillSprite2D = GetNode<Sprite2D>("Fill");

		DetectNodeType();

		if (NodeType == NodeType.Producer || NodeType == NodeType.Factory)
		{
			AddToGroup("selectable_units");
		}

		if (NodeType.Equals(NodeType.Consumer))
		{
			_questDuration = new Timer();
			_questDuration.OneShot = true;
			AddChild(_questDuration);
			_questDuration.Timeout += OnQuestDurationTimeout;
		}
		
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
		
		if (NodeType == NodeType.Producer)
		{
			AddChild(_resourceProductionTimer);
			_resourceProductionTimer.OneShot = false;
			_resourceProductionTimer.Start(1.0f/Output[0].Amount);
			_resourceProductionTimer.Timeout += ProduceOutput;
		}

		OnInputSatisfied += () => { GD.Print(this.Name + ": Input satisfied"); };
		/* if (_nodeType == NodeType.Consumer)
		{
			OnInputSatisfied += () => {};
		} */
	}

	public void RegisterConsumerFailedAction(Action action)
	{
		GD.Print("Registered funciton for consumer failed");
		_questDuration.Timeout += action;
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
			NodeType = NodeType.Factory;
		}

		else if (hasInput)
		{
			//GD.Print("Node \"" + this.Name + "\" is a Consumer");
			NodeType = NodeType.Consumer;
		}

		else if (hasOutput)
		{
			//GD.Print("Node \"" + this.Name + "\" is a Producer");
			NodeType = NodeType.Producer;
		}
		else
		{
			NodeType = NodeType.None;
			GD.PrintErr("This node is a root node " + this.Name);
		}
		GD.Print("Node \"" + this.Name + "\" is a " +  NodeType.ToString());
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

		var scale = (float)_producedResourceBuffer.Amount / 2 / Output[0].Amount;
		_fillSprite2D.Scale = new Vector2(scale, scale);

		_producedResourceBuffer.Resource = Output[0].Resource;
		if (NodeType == NodeType.Producer)
			_producedResourceBuffer.Amount = Math.Clamp(_producedResourceBuffer.Amount + 1, 0, Output[0].Amount * 2);
		if (NodeType == NodeType.Factory)
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
				
				if (NodeType == NodeType.Consumer && _questDuration != null && !_questDuration.IsStopped())
				{
					_questDuration.Stop();
					GD.Print($"Consumer {this.Name} task completed successfully!");
					OnConsumerTaskCompleted?.Invoke();
				}
				
				ResetInputInventory();
				ProduceOutput();
			}
		}
	}

	public void StartConsumerTimer()
	{
		GD.Print("Started consumer timer");
		if (NodeType != NodeType.Consumer)
		{
			GD.PrintErr("Tried calling Start Consumer Timer on " + this.Name + " but this node is not a consumer");
			return;
		}

		CheckTimer(_taskTimeSeconds);
	}
	
	public void CheckTimer(float length)
	{
		if (_questDuration.IsStopped())
		{
			GD.Print("Timer not running - starting new timer");
			_questDuration.Start(length);
		}
		else 
		{
			GD.Print($"Timer is running. Time left: {_questDuration.TimeLeft:F2} seconds" );
		}
	}

	public void OnQuestDurationTimeout()
	{
		GD.Print($"Consumer {this.Name} timer finished - task failed!");
		ResetInputInventory();
		
		// Destroy all incoming paths to this consumer
		var allPaths = GetParent()?.GetChildren().OfType<GraphPath>().ToList();
		if (allPaths != null)
		{
			foreach (var path in allPaths)
			{
				if (path.ChildNode == this)
				{
					if (path.ParentNode != null && path.ParentNode.Paths.Contains(path))
					{
						path.ParentNode.Paths.Remove(path);
					}
					path.QueueFree();
				}
			}
		}
	}

	public bool IsInsideSelectionBox(Rect2 box)
	{
		// copy 
		var rect = new Rect2();
		rect.Position = box.Position;
		rect.Size = box.Size;

		float newX = rect.Position.X;
		float newSizeX = rect.Size.X;
		if (rect.Size.X < 0)
		{
			newX = rect.Position.X + rect.Size.X;
			newSizeX = Math.Abs(rect.Size.X);
		}
		
		float newY = rect.Position.Y;
		float newSizeY = rect.Size.Y;
		if (rect.Size.Y < 0)
		{
			newY = rect.Position.Y + rect.Size.Y;
			newSizeY = Math.Abs(rect.Size.Y);
		}
		
		rect.Position = new Vector2(newX, newY);
		rect.Size = new Vector2(newSizeX, newSizeY);
		return rect.HasPoint(Position);
	}

	public void Select()
	{
		// selector.Show(); if there is a selector
		AddToGroup("selected_units");
	}

	public void Deselect()
	{
		// selector.Hide();
		RemoveFromGroup("selected_units");
	}

	public void _on_area_2d_mouse_entered()
	{
		MouseOver = true;
		_lastHovered = this;
	}

	public void _on_area_2d_mouse_exited()
	{
		MouseOver = false;
		if (_lastHovered == this)
		{
			_lastHovered = null;
		}
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
					_mouseOffset = Position - GetViewport().GetCamera2D().GetGlobalMousePosition();
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
			Position = GetViewport().GetCamera2D().GetGlobalMousePosition() + _mouseOffset;
		}

		if (Input.IsMouseButtonPressed(MouseButton.Right))
		{
			//GD.Print(Name);
			if (!_isConnecting && MouseOver)
			{
				//Start Connection
				_pathOrigin = this;
				_isConnecting = true;
				var previewScene = GD.Load<PackedScene>(PathLookup.PathPreviewPath);
				var previewInstance = previewScene.Instantiate() as PathPreview;
				AddChild(previewInstance);
				_preview = previewInstance;
				_preview!.Position = Vector2.Zero;
			}
			if (_preview != null)
			{
				_preview!.EndPosition = GetViewport().GetCamera2D().GetGlobalMousePosition();
			}
		}
		else
		{
			if (_isConnecting)
			{
				//End Connection
				_isConnecting = false;
				//Try Connection
				if (_lastHovered != null && _lastHovered != _pathOrigin)
				{
					AddConnection(_lastHovered);
				}
				//Destroy preview
				_preview.QueueFree();
				_preview = null;
				_pathOrigin = null;
			}
		}

		UpdateLabel();
	}

	private int GetInputResourceCount(ResourceAmount resource)
	{
		return Math.Clamp(resource.Amount - _inputInventory[resource.Resource], 0, resource.Amount);
	}
	
	private void UpdateLabel()
	{
		string text = "";
		if (NodeType.Equals(NodeType.Consumer) || NodeType.Equals(NodeType.Factory))
		{
			text += "IN: ";
			foreach (var re in Recource_Input)
			{
				if (re == null)
				{
					continue;
				}

				text += $"{re.Resource.ToString()}: {GetInputResourceCount(re)}/{re.Amount}";
			}
		}

		if (NodeType.Equals(NodeType.Producer) || NodeType.Equals(NodeType.Factory))
		{
			text += "\nOUT: ";
			text += $"{Output[0].Resource.ToString()}: {_producedResourceBuffer.Amount}/{Output[0].Amount}";
		}
		_statusLabel.Text = text;
	}

	private void AddConnection(I_GraphNode targetNode)
	{
		//Check if Connection Allowed
		if (CanConnect(targetNode))
		{
			var pathScene = GD.Load<PackedScene>(PathLookup.PathScenePath);
			var pathInstance = pathScene.Instantiate() as GraphPath;
			pathInstance.ChildNode = targetNode;
			pathInstance.ParentNode = _pathOrigin;
			_pathOrigin.Paths.Add(pathInstance);
			GetParent()!.AddChild(pathInstance);
			_pathOrigin.PathFinished(pathInstance);
			
			// Start consumer timer on first connection
			if (targetNode.NodeType == NodeType.Consumer)
			{
				targetNode.StartConsumerTimer();
			}
		}
	}

	private bool CanConnect(I_GraphNode targetNode)
	{
		if (targetNode == null || targetNode == _pathOrigin)
		{
			return false;
		}
		if (targetNode.NodeType == NodeType.Producer)
		{
			return false;
		}
		if (_pathOrigin.NodeType == NodeType.Consumer)
		{
			return false;
		}
		if (_pathOrigin.Paths.Select(x => x.ChildNode).Contains(targetNode))
		{
			return false;
		}
		if (targetNode.Paths.Select(x => x.ChildNode).Contains(_pathOrigin))
		{
			return false;
		}
		return true;
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
