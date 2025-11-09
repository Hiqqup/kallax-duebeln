using Godot;

[GlobalClass]
public partial class ResourceAmount : Resource
{
    public ResourceAmount() {}

    public ResourceAmount(ProductionResource resource, int amount)
    {
        Resource = resource;
        Amount = amount;
    }
    [Export] public ProductionResource Resource { get; set; } = ProductionResource.None;
    [Export] public int Amount { get; set; } = 0;
}