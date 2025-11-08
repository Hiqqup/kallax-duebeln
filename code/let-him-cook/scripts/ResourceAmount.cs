using Godot;

[GlobalClass]
public partial class ResourceAmount : Resource
{
    [Export] public ProductionResource Resource { get; set; } = ProductionResource.None;
    [Export] public int Amount { get; set; } = 0;
}