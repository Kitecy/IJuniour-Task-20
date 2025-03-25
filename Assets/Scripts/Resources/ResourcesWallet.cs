using System;
using UnityEngine;

public class ResourcesWallet : MonoBehaviour
{
    private int _value;

    public event Action<int> Changed;

    public bool TryUse(int count)
    {
        count = count < 0 ? 0 : count;

        if (CanSpend(count) == false)
            return false;

        _value -= count;

        Changed?.Invoke(_value);

        return true;
    }

    public void Add()
    {
        _value += 1;

        Changed?.Invoke(_value);
    }

    public bool CanSpend(int count)
    {
        return _value >= count;
    }
}
