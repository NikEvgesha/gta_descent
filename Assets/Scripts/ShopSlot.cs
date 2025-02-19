using UnityEngine;
using UnityEngine.UI;

public abstract class ShopSlot : MonoBehaviour
{
    [SerializeField] private Text _priceObj;
    [SerializeField] private Image _currencyImgObj;
    

    [SerializeField] private int _price;
    [SerializeField] private CurrencyType _currencyType;
    


    protected void Start()
    {
        Debug.Log("Set price " + _price + _currencyType);
        _priceObj.text = _price.ToString();
        _currencyImgObj.sprite = CurrencyManager.Instance.GetCurrencyIcon(_currencyType);
    }

    public abstract void OnClick();


}
