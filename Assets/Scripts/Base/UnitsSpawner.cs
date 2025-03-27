using UnityEngine;

public class UnitsSpawner : Spawner<Unit>
{
    [SerializeField] private DistanceChecker _distanceChecker;

    protected override void OnGetObject(Unit obj)
    {
        base.OnGetObject(obj);
        obj.SetDistanceChecker(_distanceChecker);
    }
}
