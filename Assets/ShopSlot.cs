using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Text _textObj;
    [SerializeField] private Text _priceObj;
    [SerializeField] private Image _currencyImgObj;
    [SerializeField] private Image _itemImgObj;

    [SerializeField] private string _text;
    [SerializeField] private int _amount;
    [SerializeField] private int _price;
    [SerializeField] private CurrencyType _currencyType;
    [SerializeField] private Sprite _itemImg;


    private void Start()
    {
        _textObj.text = _text;
        _priceObj.text = _price.ToString();
        _currencyImgObj.sprite = CurrencyManager.Instance.GetCurrencyIcon(_currencyType);
        _itemImgObj.sprite = _itemImg;
    }

    public void OnClick()
    {
        // обработка покупки
        CurrencyManager.Instance.AddGems(_amount);
        Debug.Log("Buy");
    }


}
