using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextUI _gems;
    [SerializeField] private TextUI _cups;

    private void OnEnable()
    {
        CurrencyManager.Instance.GemsChanged += SetGems;
        CurrencyManager.Instance.CupsChanged += SetCups;
        GameManager.Instance.LevelInProgress += ToggleGemsUI;
    }

    private void OnDisable()
    {
        CurrencyManager.Instance.GemsChanged -= SetGems;
        CurrencyManager.Instance.CupsChanged -= SetCups;
        GameManager.Instance.LevelInProgress -= ToggleGemsUI;
    }

    private void ToggleGemsUI(bool lvlInProgress)
    {
        if (_gems != null)
            _gems.gameObject.SetActive(!lvlInProgress);
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
