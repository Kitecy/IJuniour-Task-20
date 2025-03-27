using UnityEngine;

public class BaseBuilder : Generator<Base>
{
    [SerializeField] private Vector3 _offset;

    [field: SerializeField] public Transform Flag { get; private set; }

    public void SetBaseSpawner(BaseSpawner baseSpawner)
    {
        ObjectsSpawner = baseSpawner;
    }

    public void SetFlag(Vector3 position)
    {
        Flag.gameObject.SetActive(true);
        Flag.position = position;
    }

    public bool TryCreate(ResourcesWallet wallet, out Base @base)
    {
        @base = null;

        if (wallet.TryUse(Price) == false)
            return false;

        @base = Create();
        return true;
    }

    private Base Create()
    {
        Base @base = ObjectsSpawner.GetObject();
        @base.transform.position = Flag.position + _offset;
        @base.gameObject.SetActive(false);
        Flag.gameObject.SetActive(false);
        return @base;
    }
}
