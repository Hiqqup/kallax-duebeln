using Godot;
using System;
using Godot.Collections;

public partial class NodeStatusLabel : PanelContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	[Export] private VBoxContainer _inContainer;
	[Export] private HBoxContainer _arrowContainer;
	[Export] private VBoxContainer _outContainer;
	[Export] private Label _inLabel;
	[Export] private Label _outLabel;
	[Export] private RichTextLabel _timerLabel;

	public void UpdateLabel(I_GraphNode node)
	{
		if (node.MouseOver == true || Input.IsActionJustPressed("show_all_tooltips"))
		{
			Show();
		}else if (node.MouseOver == false && (Input.IsActionJustReleased("show_all_tooltips") || !Input.IsActionPressed("show_all_tooltips")))
		{
			Hide();
		}
		
		var nodeType = node.NodeType;
		if (nodeType == NodeType.Factory)
		{
			_arrowContainer.Visible = true;
		}
		else
		{
			_arrowContainer.Visible = false;
		}

		if (nodeType == NodeType.Consumer)
		{
			_outContainer.Visible = false;
		}
		else
		{
			_outContainer.Visible = true;
		}
		
		if (nodeType == NodeType.Producer)
		{
			_inContainer.Visible = false;
		}
		else
		{
			_inContainer.Visible = true;
		}

		if (node.QuestDuration != null)
		{
			if (!node.QuestDuration.IsStopped())
			{
				_timerLabel.Visible = true;
				_timerLabel.Text = $"Time left: [color=red]{(int) node.QuestDuration.TimeLeft+ 1}[/color]";
			}
			else
			{
				_timerLabel.Visible = false;
			}
		}
		else
		{
			_timerLabel.Visible = false;
		}
		

		string inText = "";
		foreach (ResourceAmount resource in node.Recource_Input)
		{
			if (resource == null)
			{
				continue;
			}
			inText += $"{resource.Resource.ToString()}: {GetInputResourceCount(resource, node)}/{resource.Amount} \n";
		}
		_inLabel.Text = inText;

		var outText = "";
		if (node.Output is { Count: > 0 })
		{
			outText += $"{node.Output[0].Resource.ToString()}: {node.ProducedResourceBuffer.Amount}/{node.Output[0].Amount} \n";
		}
		_outLabel.Text = outText;
	}
	
	private int GetInputResourceCount(ResourceAmount resource, I_GraphNode node)
	{
		if (node.InputInventory.ContainsKey(resource.Resource))
		{
			return Math.Clamp(resource.Amount - node.InputInventory[resource.Resource], 0, resource.Amount);
		}
		return 0;
	}
}
