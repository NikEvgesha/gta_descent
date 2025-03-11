using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextUI _gems;
    [SerializeField] private TextUI _cups;

    private void Start()
    {
        Debug.Log("Currency UI start");
        CurrencyManager.Instance.GemsChanged += SetGems;
        CurrencyManager.Instance.CupsChanged += SetCups;
        SetGems(CurrencyManager.Instance.Gems);
        SetCups(CurrencyManager.Instance.Cups);
        //GameManager.Instance.LevelInProgress += ToggleGemsUI;
    }

    private void OnDisable()
    {
        CurrencyManager.Instance.GemsChanged -= SetGems;
        CurrencyManager.Instance.CupsChanged -= SetCups;
        //GameManager.Instance.LevelInProgress -= ToggleGemsUI;
    }

    private void ToggleGemsUI(bool lvlInProgress)
    {
        if (_gems != null)
            _gems.gameObject.SetActive(!lvlInProgress);
    }

    private void SetGems(int newAmount)
    {
        Debug.Log("Set UI gems: " + newAmount);
        _gems.Set(newAmount.ToString());
    }

    private void SetCups(int newAmount)
    {
        Debug.Log("Set UI cups: " + newAmount);
        _cups.Set(newAmount.ToString());
    }
}
