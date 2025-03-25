using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private ResourcesWallet _wallet;
    [SerializeField] private UnitGenerator _unitGenerator;
    [SerializeField] private BaseBuilder _builder;
    [SerializeField] private List<Unit> _units;
    [SerializeField] private ResourceSpawner _spawner;
    [SerializeField] private ResourceScanner _scanner;
    [SerializeField] private ResourcesQueue _resourceQueue;

    private List<Unit> _freeUnits;
    private OccupationType _generatingType;

    private enum OccupationType
    {
        Unit,
        Base
    }

    [field: SerializeField] public Transform ArrivalPoint { get; private set; }

    private void Awake()
    {
        _scanner.Scanned += OnResourcesScanned;
        _freeUnits = new(_units);
    }

    private void OnEnable()
    {
        foreach (Unit unit in _units)
        {
            unit.ReachedResource += OnUnitReachedResource;
            unit.ReachedBase += OnUnitReachedBase;
        }
    }

    private void OnDisable()
    {
        foreach (Unit unit in _units)
        {
            unit.ReachedResource -= OnUnitReachedResource;
            unit.ReachedBase -= OnUnitReachedBase;
        }
    }

    public void SetResourcesQueue(ResourcesQueue resourcesQueue)
    {
        _resourceQueue = resourcesQueue;
    }

    public void SetResourceSpawner(ResourceSpawner resourceSpawner)
    {
        _spawner = resourceSpawner;
    }

    public void SetUnitsSpawner(UnitsSpawner unitsSpawner)
    {
        _unitGenerator.SetUnitsSpawner(unitsSpawner);
    }

    public void SetBaseSpawner(BaseSpawner baseSpawner)
    {
        _builder.SetBaseSpawner(baseSpawner);
    }

    public void ReplaceFlag(Vector3 position)
    {
        _builder.SetFlag(position);
        _generatingType = OccupationType.Base;
    }

    public void AddUnit(Unit unit)
    {
        if (_units.Contains(unit))
            return;

        unit.ReachedResource += OnUnitReachedResource;
        unit.ReachedBase += OnUnitReachedBase;

        _units.Add(unit);
        _freeUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (_units.Contains(unit) == false)
            return;

        _units.Remove(unit);

        unit.ReachedBase -= OnUnitReachedBase;
        unit.ReachedResource -= OnUnitReachedResource;

        if (_freeUnits.Contains(unit))
            _freeUnits.Remove(unit);
    }

    private void OnResourcesScanned(List<Resource> resources)
    {
        if (resources.Count == 0 || _freeUnits.Count == 0)
            return;

        for (int i = 0; i < resources.Count && _freeUnits.Count > 0; i++)
            if (_resourceQueue.TryAddResource(resources[i]))
                SendForResource(resources[i]);
    }

    private void OnUnitReachedResource(Unit unit)
    {
        unit.GoToBase(ArrivalPoint);
    }

    private void OnUnitReachedBase(Unit unit)
    {
        Resource collectedResource = unit.GiveResource();
        _spawner.ReleaseObject(collectedResource);
        _resourceQueue.RemoveResource(collectedResource);

        _wallet.Add();

        _freeUnits.Add(unit);

        switch (_generatingType)
        {
            case OccupationType.Unit:
                _unitGenerator.TryCreate(_wallet, this);
                break;

            case OccupationType.Base:
                StartCreatingBase();
                break;
        }
    }

    private void StartCreatingBase()
    {
        if (_builder.CanBuy(_wallet) == false)
            return;

        if (_freeUnits.Count > 0 && _generatingType == OccupationType.Base)
            SendToCreateBase();
    }

    private void SendForResource(Resource resource)
    {
        Unit unit = _freeUnits.First();
        _freeUnits.Remove(unit);

        unit.GoToResources(resource);
    }

    private void SendToCreateBase()
    {
        Unit unit = _freeUnits.First();
        _generatingType = OccupationType.Unit;
        _freeUnits.Remove(unit);

        unit.GoToPoint(_builder.Flag, OnUnitCreatedBase);
    }

    private void OnUnitCreatedBase(Unit unit)
    {
        _builder.TryCreate(_wallet, out Base @base);

        RemoveUnit(unit);
        @base.AddUnit(unit);
    }
}