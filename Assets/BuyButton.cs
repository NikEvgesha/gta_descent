using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private GameObject _buttonObj;
    [SerializeField] private Text _priceText;
    [SerializeField] private Image _currencyImg;

    [SerializeField] private Color _gemColor;
    [SerializeField] private Color _cupsColor;
    [SerializeField] private Color _realColor;

    private ShopItemData _currentItemData;
    private IPurchasable _shopSlot;


    public void ShowButton(ShopItemData data, IPurchasable shopSlot)
    {
        _shopSlot = shopSlot;
        _currentItemData = data;
        _buttonObj.SetActive(true);
        _priceText.text = data.Price.ToString();
        _currencyImg.sprite = CurrencyManager.Instance.GetCurrencyIcon(data.CurrencyType);
    }

    public void HideButton()
    {
        _buttonObj.SetActive(false);
    }


    public void OnClick()
    {
        _shopSlot.OnBuyButtonClick();
    }
}
