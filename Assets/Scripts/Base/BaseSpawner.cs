using UnityEngine;

public class BaseSpawner : Spawner<Base>
{
    [SerializeField] private ResourcesQueue _resourcesQueue;
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private UnitsSpawner _unitsSpawner;

    protected override void OnGetObject(Base @base)
    {
        base.OnGetObject(@base);

        @base.SetResourcesQueue(_resourcesQueue);
        @base.SetBaseSpawner(this);
        @base.SetUnitsSpawner(_unitsSpawner);
    }
}
