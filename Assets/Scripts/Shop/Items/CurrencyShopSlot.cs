using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopSlot : ShopSlot
{

    [SerializeField] private Text _textObj;

    [SerializeField] private CurrencyPackData _packData;

/*    private new void Start()
    {
        base.Start();
        _textObj.text = _packData.Description;
        _itemImgObj.sprite = _shopItemData.ItemIMG;
    }*/

    public void Init(CurrencyPackData data)
    {
        base.Init(data);
        _packData = data;
        _textObj.text = _packData.Description;

        if (data.PurchaseReward)
        {
            PurchaseData PurchaseData = PurchasesManager.Instance.GetPurchaseData(data.PurchaseRewardName.ToString());
            if (PurchaseData != null)
            {
                Debug.Log($"Item: {PurchaseData.Title}, Price: {PurchaseData.Price}");
                _textObj.text = PurchaseData.Title;
                _priceObj.text = PurchaseData.Price;
            }
        }

        _itemImgObj.sprite = _packData.ItemIMG;
    }

    public override void OnClick()
    {
        // обработка покупки
        Shop.Instance.TryBuy(_packData);
    }


}
