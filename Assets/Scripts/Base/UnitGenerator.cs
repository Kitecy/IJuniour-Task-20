public class UnitGenerator : Generator<Unit>
{
    public void SetUnitsSpawner(UnitsSpawner spawner)
    {
        ObjectsSpawner = spawner;
    }

    public bool TryCreate(ResourcesWallet wallet, Base @base)
    {
        if (CanBuy(wallet) == false)
            return false;

        Create(@base);

        wallet.TryUse(Price);
        return true;
    }

    private void Create(Base @base)
    {
        Unit unit = ObjectsSpawner.GetObject();
        @base.AddUnit(unit);
        unit.transform.position = @base.ArrivalPoint.position;
    }
}
