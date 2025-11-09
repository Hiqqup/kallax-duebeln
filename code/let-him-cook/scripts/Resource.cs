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
    /**
     * Gets a random item of the Production Resources
     */
    public static ProductionResource GetRandom()
    {
        return (ProductionResource)GD.RandRange((int)ProductionResource.None+1, (int)ProductionResource.END-1);
    }
}