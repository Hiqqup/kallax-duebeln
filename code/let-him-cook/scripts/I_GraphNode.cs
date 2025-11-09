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
	private CollisionShape2D _collisionShape2D;
	private Sprite2D _fillSprite2D;
	private Sprite2D _contentSprite2D;
	private Label _statusLabel;

	private readonly float _taskTimeSeconds = 15.0f;

	private Timer _questDuration;

	private Camera2D _cam;

	[Export] public Sprite2D circleSprite;
	[Export] public Texture2D squircleTexture;
	[Export] public Texture2D diamondTexture;
	[Export] public Texture2D circleTexture;


	[Export] public Sprite2D circleFillSprite;
	[Export] public Texture2D squircleFillTexture;
	[Export] public Texture2D diamondFillTexture;
	[Export] public Texture2D circleFillTexture;

	private static bool _anyUnitBeingDragged = false;

	public I_GraphNode()
	{

	}
	public I_GraphNode(Array<ResourceAmount> inputs, Array<ResourceAmount> outputs)
	{
		Recource_Input = inputs;
		Output = outputs;
	}

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
		if (_fillSprite2D == null) GD.Print("Fill sprite is not valid");
		_contentSprite2D = GetNode<Sprite2D>("Sprite2D");

		DetectNodeType();
		SetupResourceTexture();

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

		UpdateFill();

		if (NodeType == NodeType.Producer)
		{
			AddChild(_resourceProductionTimer);
			_resourceProductionTimer.OneShot = false;
			_resourceProductionTimer.Start(1.0f / Output[0].Amount);
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

	public void AddPath(GraphPath path)
	{
		_pathQueue.Enqueue(path);

		UpdateFill();
	}
	public void PathFinished(GraphPath path)
	{
		AddPath(path);
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

		UpdateFill();
		//GD.Print(path.Name + " started transporting " + _producedResourceBuffer.Resource.ToString());
	}

	private void DetectNodeType()
	{
		bool hasInput = Recource_Input is { Count: > 0 };
		bool hasOutput = Output != null && Output.Count > 0 && Output[0].Resource != ProductionResource.None;
		if (hasInput && hasOutput)
		{
			NodeType = NodeType.Factory;
			circleSprite.Texture = diamondTexture;
			circleFillSprite.Texture = diamondFillTexture;
		}

		else if (hasInput)
		{
			NodeType = NodeType.Consumer;
			circleSprite.Texture = squircleTexture;
			circleFillSprite.Texture = squircleFillTexture;
		}

		else if (hasOutput)
		{
			NodeType = NodeType.Producer;
			circleSprite.Texture = circleTexture;
			circleFillSprite.Texture = circleFillTexture;
		}
		else
		{
			NodeType = NodeType.None;
			GD.PrintErr("This node is a root node " + this.Name);
		}
		//GD.Print("Node \"" + this.Name + "\" is a " +  NodeType.ToString());
	}

	void SetupResourceTexture()
	{
		string resourcePath = "";
		if (NodeType == NodeType.Producer || NodeType == NodeType.Factory)
		{
			if (Output.Count > 0)
				resourcePath = SpriteLookup.MapResourceToFile(Output[0].Resource);
		}

		if (NodeType == NodeType.Consumer)
		{
			if (Recource_Input.Count > 0)
				resourcePath = SpriteLookup.MapResourceToFile(Recource_Input[0].Resource);
		}

		if (resourcePath != "")
		{
			var texture = GD.Load<Texture2D>(resourcePath);
			_contentSprite2D.Texture = texture;

			if (texture != null)
			{
				Vector2 textureResolution = texture.GetSize();
				float nodeSize = Constants.NodeDiameter;

				float padding = 0.9f;
				float scaleX = nodeSize / textureResolution.X;
				float scaleY = nodeSize / textureResolution.Y;
				float scaleFactor = Math.Min(scaleX, scaleY) * padding;
				_contentSprite2D.Scale = new Vector2(scaleFactor, scaleFactor);
			}
		}

	}

	private void ResetInputInventory()
	{
		if (Recource_Input == null || Recource_Input.Count <= 0) return;

		foreach (var resourceAmount in Recource_Input)
		{
			if (resourceAmount?.Resource == null || resourceAmount.Resource == ProductionResource.None || resourceAmount.Amount <= 0) continue;

			_inputInventory[resourceAmount.Resource] = resourceAmount.Amount;
		}
		UpdateFill();
	}

	private int GetRemainingNeededResourceAmount()
	{
		return _inputInventory.Keys.Sum(input => Math.Max(_inputInventory[input], 0));
	}

	private int GetTotalNeededResourceAmount()
	{
		return Recource_Input.Sum(resource => resource.Amount);
	}

	private void UpdateFill()
	{
		var scale = 1.0f;
		if (NodeType == NodeType.Producer)
		{
			scale = (float)_producedResourceBuffer.Amount / 2 / Output[0].Amount;
		}

		if (NodeType == NodeType.Factory)
		{
			scale = 0.0f;
		}

		if (NodeType == NodeType.Consumer)
		{
			//GD.Print("Remaining needed amount " + GetRemainingNeededResourceAmount() + " total needed amount " + GetTotalNeededResourceAmount());
			scale = 1.0f - ((float)GetRemainingNeededResourceAmount() /
					 GetTotalNeededResourceAmount());
		}
		_fillSprite2D.Scale = new Vector2(scale, scale);

	}

	public void ProduceOutput()
	{
		if (Output == null || Output.Count == 0 || Output[0].Resource == ProductionResource.None) return;

		_producedResourceBuffer.Resource = Output[0].Resource;
		if (NodeType == NodeType.Producer)
			_producedResourceBuffer.Amount = Math.Clamp(_producedResourceBuffer.Amount + 1, 0, Output[0].Amount * 2);
		if (NodeType == NodeType.Factory)
			_producedResourceBuffer.Amount = Math.Clamp(_producedResourceBuffer.Amount + Output[0].Amount, 0, Output[0].Amount * 2);

		UpdateFill();

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
			UpdateFill();
			if (IsInputSatisfied())
			{
				OnInputSatisfied?.Invoke();

				if (NodeType == NodeType.Consumer && _questDuration != null && !_questDuration.IsStopped())
				{
					_questDuration.Stop();
					GD.Print($"Consumer {this.Name} task completed successfully!");
					OnConsumerTaskCompleted?.Invoke();

					SpawnRewardsScreen();

					RemoveAllIncomingPaths();
				}

				ResetInputInventory();
				ProduceOutput();
			}
		}
	}

	private void SpawnRewardsScreen()
	{
		// Start the reward selection
		GameManager.Instance.reward.StartRewardSelection((selectedRewardIndex) =>
		{
			GD.Print($"Player selected reward {selectedRewardIndex}");
			// Handle reward logic

			// Clean up the reward
			this.QueueFree();
		});
	}

	public void StartConsumerTimer()
	{
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
			GD.Print($"Timer is running. Time left: {_questDuration.TimeLeft:F2} seconds");
		}
	}

	private void RemoveAllIncomingPaths()
	{
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

	public void OnQuestDurationTimeout()
	{
		GD.Print($"Consumer {this.Name} timer finished - task failed!");
		ResetInputInventory();
		RemoveAllIncomingPaths();

	}

	#region Unit Selection

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && MouseOver)
		{
			// Only handle individual unit clicks if we're NOT box selecting
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				// Check if this unit is already selected (part of a group)
				if (IsInGroup("selected_units"))
				{
					// Start dragging all selected units
					_mouseOffset = Position - GetViewport().GetCamera2D().GetGlobalMousePosition();
					FollowMouse = true;
					_anyUnitBeingDragged = true;

					// Make all selected units follow
					foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selected_units").Cast<I_GraphNode>())
					{
						if (unit != this)
						{
							unit.StartFollowing(unit.Position - GetViewport().GetCamera2D().GetGlobalMousePosition());
						}
					}

					// Consume the event so UnitSelector doesn't start box selecting
					GetViewport().SetInputAsHandled();
				}
				else
				{
					// Unit is not selected - select it and start dragging immediately
					// First deselect all other units
					foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selected_units").Cast<I_GraphNode>().ToList())
					{
						unit.Deselect();
					}

					// Select this unit
					Select();

					// Start dragging
					_mouseOffset = Position - GetViewport().GetCamera2D().GetGlobalMousePosition();
					FollowMouse = true;
					_anyUnitBeingDragged = true;

					// Consume the event so UnitSelector doesn't start box selecting
					GetViewport().SetInputAsHandled();
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Left && !mouseEvent.Pressed)
			{
				if (FollowMouse)
				{
					// Stop dragging
					FollowMouse = false;
					_anyUnitBeingDragged = false;

					// Stop all selected units and deselect them
					foreach (I_GraphNode unit in GetTree().GetNodesInGroup("selected_units").Cast<I_GraphNode>())
					{
						unit.StopFollowing();
						unit.Deselect();
					}
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

	public void StartFollowing(Vector2 offset)
	{
		_mouseOffset = offset;
		FollowMouse = true;
	}

	public void StopFollowing()
	{
		FollowMouse = false;
	}

	public static bool IsAnyUnitBeingDragged()
	{
		return _anyUnitBeingDragged;
	}

	#endregion





	public void _on_area_2d_mouse_entered()
	{
		MouseOver = true;
		_lastHovered = this;
	}

	public void _on_area_2d_mouse_exited()
	{
		MouseOver = false;
		// Don't clear _lastHovered if we're in the middle of a connection - this prevents
		// the issue where moving a node during connection clears the hover state
		if (_lastHovered == this && !_isConnecting)
		{
			_lastHovered = null;
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

				// Try to find target node - use _lastHovered if available, otherwise do distance check
				I_GraphNode targetNode = _lastHovered;
				if (targetNode == null && _pathOrigin != null)
				{
					targetNode = _pathOrigin.FindNodeAtMousePosition();
				}

				//Try Connection
				if (targetNode != null && targetNode != _pathOrigin)
				{
					AddConnection(targetNode);
				}
				//Destroy preview
				_preview.QueueFree();
				_preview = null;
				_pathOrigin = null;
				_lastHovered = null; // Clear after connection attempt
			}
		}

		UpdateLabel();
	}

	private int GetInputResourceCount(ResourceAmount resource)
	{
		if (_inputInventory.ContainsKey(resource.Resource))
			return Math.Clamp(resource.Amount - _inputInventory[resource.Resource], 0, resource.Amount);
		return 0;
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
			_pathOrigin.AddPath(pathInstance);

			// Start consumer timer on first connection
			if (targetNode.NodeType == NodeType.Consumer)
			{
				targetNode.StartConsumerTimer();
			}
		}
	}

	private I_GraphNode FindNodeAtMousePosition()
	{
		var mousePos = GetViewport().GetCamera2D().GetGlobalMousePosition();

		// Get all I_GraphNode instances from the parent (Level) node
		var allNodes = GetParent()?.GetChildren().OfType<I_GraphNode>()
			.Where(node => node != null && node != this && node != _pathOrigin)
			.ToList() ?? new List<I_GraphNode>();

		I_GraphNode closestNode = null;
		float closestDistance = float.MaxValue;
		const float maxConnectionDistance = 150.0f; // Match the collision radius

		foreach (var node in allNodes)
		{
			var distance = mousePos.DistanceTo(node.Position);
			if (distance < closestDistance && distance <= maxConnectionDistance)
			{
				closestDistance = distance;
				closestNode = node;
			}
		}

		return closestNode;
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
