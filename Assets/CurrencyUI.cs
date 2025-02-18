using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextUI _gems;
    [SerializeField] private TextUI _cups;

    private void OnEnable()
    {
        CurrencyManager.Instance.GemsChanged += SetGems;
        CurrencyManager.Instance.CupsChanged += SetCups;
    }

    private void SetGems(int newAmount)
    {
        _gems.Set(newAmount.ToString());
    }

    private void SetCups(int newAmount)
    {
        _cups.Set(newAmount.ToString());
    }
}
