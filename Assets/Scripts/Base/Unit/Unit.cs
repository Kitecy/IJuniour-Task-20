using System;
using UnityEngine;

[RequireComponent(typeof(UnitMovement), typeof(Collector))]
public class Unit : MonoBehaviour
{
    private Collector _collector;
    private UnitMovement _movement;
    private Resource _targetResource;

    private Action<Unit> _onReachedCallback;

    public event Action<Unit> ReachedResource;
    public event Action<Unit> ReachedBase;

    private void Awake()
    {
        _collector = GetComponent<Collector>();
        _movement = GetComponent<UnitMovement>();
    }

    public void SetDistanceChecker(DistanceChecker distanceChecker)
    {
        _collector.SetDistanceChecker(distanceChecker);
    }

    public void GoToPoint(Transform point, Action<Unit> onReachedCallback = null)
    {
        if (_movement.IsBusy)
            return;

        _movement.SetTarget(point);

        if (onReachedCallback == null)
            return;

        _onReachedCallback = onReachedCallback;
        _movement.Reached += OnReachedCallback;
    }

    public void GoToResources(Resource resource)
    {
        if (_movement.IsBusy)
            return;

        _targetResource = resource;

        _movement.SetTarget(resource.ArrivalPoint);
        _movement.Reached += OnReachedResource;
    }

    public void GoToBase(Transform @base)
    {
        if (_movement.IsBusy)
            return;

        _movement.SetTarget(@base);
        _movement.Reached += OnReachedBase;
    }

    public Resource GiveResource()
    {
        _targetResource = null;
        return _collector.GiveResource();
    }

    private void OnReachedCallback()
    {
        _movement.Reached -= OnReachedCallback;
        _onReachedCallback?.Invoke(this);
        _onReachedCallback = null;
    }

    private void OnReachedResource()
    {
        _movement.Reached -= OnReachedResource;

        _collector.Take(_targetResource);

        ReachedResource?.Invoke(this);
    }

    private void OnReachedBase()
    {
        _movement.Reached -= OnReachedBase;
        ReachedBase?.Invoke(this);
    }
}
