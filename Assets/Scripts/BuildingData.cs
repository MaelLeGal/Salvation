[System.Serializable]
public class BuildingData
{
    public string Name;

    public bool PlaceableOnGrass;
    public bool PlaceableOnNeutral;
    public bool PlaceableOnDryGround;

    public Building.Costs ConstructionCosts;
    public Building.Costs TickCosts;
    public Building.Costs TickProductions;

    public int Capacity;

    public InputManager.ConstructEventArgs ConstructEvent;

    public BuildingData(bool pog, bool pon, bool podg, Building.Costs cc, Building.Costs tc, Building.Costs tp, int c, InputManager.ConstructEventArgs ce)
    {
        PlaceableOnGrass = pog;
        PlaceableOnNeutral = pon;
        PlaceableOnDryGround = podg;

        ConstructionCosts = cc;
        TickCosts = tc;
        TickProductions = tp;

        Capacity = c;

        ConstructEvent = ce;
    }
}
