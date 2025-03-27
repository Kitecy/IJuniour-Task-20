using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private ResourceSpawner _spawner;

    public event Action Claimed;

    [field: SerializeField] public Transform ArrivalPoint { get; private set; }

    public void Claim()
    {
        Claimed?.Invoke();
    }

    public void SetSpawner(ResourceSpawner spawner)
    {
        _spawner = spawner;
    }

    public void Dispose()
    {
        _spawner.ReleaseObject(this);
    }
}
