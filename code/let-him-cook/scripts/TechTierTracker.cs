using Godot;
using System;
using System.Collections.Generic;

public partial class TechTierTracker
{
    public int MaxUnlockedTier = 1;

    private List<ProductionResource> alreadyProduced = new List<ProductionResource>();
    public void updateItemsProduced(ProductionResource newResource)
    {
        if (alreadyProduced.Contains(newResource))
        {
            return;
        }
        alreadyProduced.Add(newResource);
        int tierOfNewItem = ProductionResourceExtensions.TierMap[newResource];
        if (tierOfNewItem > MaxUnlockedTier)
        {
            MaxUnlockedTier = tierOfNewItem;
        }
    }
}
