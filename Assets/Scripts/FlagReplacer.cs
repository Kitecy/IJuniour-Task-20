using System;
using UnityEngine;

public class FlagReplacer : MonoBehaviour
{
    private Base _selected;

    public bool IsBusy => _selected != null;

    public void SelectBase(Base @base)
    {
        if (IsBusy)
            throw new InvalidOperationException();

        _selected = @base;
    }

    public void ReplaceFlag(Vector3 newPosition)
    {
        if (_selected == null)
            return;

        _selected.ReplaceFlag(newPosition);
        _selected = null;
    }
}
