using UnityEngine;

public abstract class Generator<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected Spawner<T> ObjectsSpawner;
    [SerializeField] protected int Price;

    public bool CanBuy(ResourcesWallet wallet) =>
        wallet.CanSpend(Price);
}
