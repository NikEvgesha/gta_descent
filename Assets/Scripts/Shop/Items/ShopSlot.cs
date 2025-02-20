using UnityEngine;
using UnityEngine.UI;

public abstract class ShopSlot : MonoBehaviour
{
    [SerializeField] protected Text _priceObj;
    [SerializeField] protected Image _currencyImgObj;
    [SerializeField] protected Image _itemImgObj;

    /*    protected void Start()
        {
            _priceObj.text = _shopItemData.Price.ToString();
            _currencyImgObj.sprite = CurrencyManager.Instance.GetCurrencyIcon(_shopItemData.CurrencyType);
        }*/

    public void Init(ShopItemData data)
    {
        _priceObj.text = data.Price.ToString();
        _currencyImgObj.sprite = CurrencyManager.Instance.GetCurrencyIcon(data.CurrencyType);
    }

    public abstract void OnClick();

}
