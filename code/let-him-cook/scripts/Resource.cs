using Godot;
using Godot.Collections;
public enum ProductionResource
{
    None = 0,
    Dowel = 1,
    PointyScrew = 2,
    ScrewPlug= 3,
    DoubleEndedScrew = 4,
    Hinge = 5,
    Plank =6,
    Kallax1x1 = 7,
    Kallax1x2 = 8,
    Kallax2x2 = 9,
    kallax2x4 = 10,
    END = 11
}
public static class ProductionResourceExtensions
{
    
    public static Dictionary<ProductionResource, int> TierMap = new Dictionary<ProductionResource, int>()
    {
        {ProductionResource.Dowel , 1},
        {ProductionResource.PointyScrew , 1},
        {ProductionResource.ScrewPlug, 1},
        {ProductionResource.DoubleEndedScrew , 2},
        {ProductionResource.Hinge , 2},
        {ProductionResource.Plank, 2},
        {ProductionResource.Kallax1x1 , 3},
        {ProductionResource.Kallax1x2 , 3},
        {ProductionResource.Kallax2x2 , 3},
        {ProductionResource.kallax2x4 , 4},
    };        
    /**
     * Gets a random item of the Production Resources
     */
    public static ProductionResource GetRandom()
    {
        return (ProductionResource)GD.RandRange((int)ProductionResource.None+1, (int)ProductionResource.END-1);
    }

    public static ProductionResource GetRandomT1Resource()
    {
        Array<ProductionResource> productionResources = new Array<ProductionResource>();
        productionResources.Add(ProductionResource.Dowel);
        productionResources.Add(ProductionResource.PointyScrew);
        productionResources.Add(ProductionResource.ScrewPlug);
        return productionResources[GD.RandRange(0, productionResources.Count-1)];
    }

    public static ProductionResource GetRandomT2Resource()
    {
        Array<ProductionResource> productionResources = new Array<ProductionResource>();
        productionResources.Add(ProductionResource.DoubleEndedScrew);
        productionResources.Add(ProductionResource.Hinge);
        productionResources.Add(ProductionResource.Plank);
        return productionResources[GD.RandRange(0, productionResources.Count-1)];
    }

    public static ProductionResource GetRandomT3Resource()
    {
        Array<ProductionResource> productionResources = new Array<ProductionResource>();
        productionResources.Add(ProductionResource.Kallax1x1);
        productionResources.Add(ProductionResource.Kallax1x2);
        productionResources.Add(ProductionResource.Kallax2x2);
        return productionResources[GD.RandRange(0, productionResources.Count-1)];
    }

    public static ProductionResource GetRandomT4Resource()
    {
        Array<ProductionResource> productionResources = new Array<ProductionResource>();
        productionResources.Add(ProductionResource.kallax2x4);
        return productionResources[GD.RandRange(0, productionResources.Count-1)];
    }

    public static ProductionResource GetRandomTierResource(int tier)
    {
        return tier switch
        {
            1 => GetRandomT1Resource(),
            2 => GetRandomT2Resource(),
            3 => GetRandomT3Resource(),
            4 => GetRandomT4Resource(),
            _ => GetRandomT1Resource()
        };
    }
}