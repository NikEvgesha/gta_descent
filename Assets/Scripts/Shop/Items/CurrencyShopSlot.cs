using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopSlot : ShopSlot
{

    [SerializeField] private Text _textObj;

    [SerializeField] private CurrencyPackData _packData;

    [SerializeField] private GameObject _ads;
    [SerializeField] private GameObject _assets;
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
        _itemImgObj.sprite = _packData.ItemIMG;
        if (data.PurchaseReward)
        {
            PurchaseData PurchaseData = PurchasesManager.Instance.GetPurchaseData(data.PurchaseRewardName.ToString());
            if (PurchaseData != null)
            {
                Debug.Log($"Item: {PurchaseData.Title}, Price: {PurchaseData.Price}");
                //_textObj.text = PurchaseData.Title;
                string number = Regex.Match(PurchaseData.Price, @"\d+\.?\d*").Value; // "19.99"
                _priceObj.text = number;
            }
        }


        _ads.SetActive(data.AdsReward);
        _priceObj.gameObject.SetActive(!data.AdsReward);
    }
    public void InitImage(Sprite imageCurr)
    {
        _currencyImgObj.sprite = imageCurr;
    }
    public override void OnClick()
    {
        // обработка покупки
        Shop.Instance.TryBuy(_packData);
    }


}
