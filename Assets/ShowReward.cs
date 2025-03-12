using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowReward : MonoBehaviour
{
    [SerializeField] GameObject _panel;
    [SerializeField] GameObject _currencyCupRevard;
    [SerializeField] GameObject _currencyGemRevard;
    [SerializeField] Text _currencyCupCount;
    [SerializeField] Text _currencyGemCount;
    public void SetRevard(bool useGem, List<CurrencyRewardData> rewards)
    {
        _panel.SetActive(true);
        _currencyGemRevard.SetActive(useGem);
        foreach (var item in rewards)
        {
            switch (item.CurrencyType)
            {
                case CurrencyType.Cups:
                    {
                        _currencyCupCount.text = item.Amount.ToString();
                        break;
                    }
                case CurrencyType.Gems:
                    {
                        _currencyGemCount.text = item.Amount.ToString();
                        break;
                    }
            }
        }
        //string CupCount,string GemCount = "";
    }
    private void OnEnable()
    {
        GameManager.Instance.SetPause(true);
        GameManager.Instance.ShowCursor();


    }
    private void OnDisable()
    {
        GameManager.Instance.SetPause(false);
        GameManager.Instance.HideCursor();
    }
}
