using TMPro;
using UnityEngine;

public class ResourcesWalletView : MonoBehaviour
{
    [SerializeField] private ResourcesWallet _wallet;
    [SerializeField] private TMP_Text _valueText;

    private void OnEnable()
    {
        _wallet.Changed += OnChanged;
    }

    private void OnDisable()
    {
        _wallet.Changed -= OnChanged;
    }

    private void OnChanged(int value)
    {
        _valueText.text = value.ToString();
    }
}
