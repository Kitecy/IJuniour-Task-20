using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Base : MonoBehaviour
{
    private const int MinUnitsCorCreatingBase = 1;

    [SerializeField] private ResourcesWallet _wallet;
    [SerializeField] private UnitGenerator _unitsGenerator;
    [SerializeField] private BaseBuilder _builder;
    [SerializeField] private List<Unit> _units;
    [SerializeField] private ResourceScanner _scanner;
    [SerializeField] private ResourcesQueue _resourceQueue;

    private List<Unit> _freeUnits;
    private OccupationType _generatingType;
    private Base _createdBase;

    private enum OccupationType
    {
        Unit,
        Base
    }

    [field: SerializeField] public Transform ArrivalPoint { get; private set; }

    private void Awake()
    {
        _freeUnits = new(_units);
    }

    private void OnEnable()
    {
        _scanner.Scanned += OnResourcesScanned;

        foreach (Unit unit in _units)
        {
            unit.ReachedResource += OnUnitReachedResource;
            unit.ReachedBase += OnUnitReachedBase;
        }
    }

    private void OnDisable()
    {
        _scanner.Scanned -= OnResourcesScanned;

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

    public void SetUnitsSpawner(UnitsSpawner unitsSpawner)
    {
        _unitsGenerator.SetUnitsSpawner(unitsSpawner);
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

    private void SendForResource(Resource resource)
    {
        Unit unit = _freeUnits.First();
        _freeUnits.Remove(unit);

        unit.GoToResources(resource);
    }

    private void OnUnitReachedResource(Unit unit)
    {
        unit.GoToBase(ArrivalPoint);
    }

    private void OnUnitReachedBase(Unit unit)
    {
        Resource collectedResource = unit.GiveResource();
        _resourceQueue.RemoveResource(collectedResource);
        collectedResource.Dispose();

        _wallet.Add();

        _freeUnits.Add(unit);

        switch (_generatingType)
        {
            case OccupationType.Unit:
                if (_unitsGenerator.TryCreate(_wallet, this) == false)
                    return;
                break;

            case OccupationType.Base:
                StartCreatingBase();
                break;
        }
    }

    private void StartCreatingBase()
    {
        if (_builder.TryCreate(_wallet, out _createdBase) == false || _units.Count <= MinUnitsCorCreatingBase)
            return;

        if (_freeUnits.Count > 0)
            SendToCreateBase();
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
        RemoveUnit(unit);
        _createdBase.AddUnit(unit);
        _createdBase.gameObject.SetActive(true);
        _createdBase = null;
    }
}