using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct CurrencyRewardData
{
    public int Amount;
    public CurrencyType CurrencyType;
}

public class CurrencyShopSlot : ShopSlot
{

    [SerializeField] private Text _textObj;
    [SerializeField] private Image _itemImgObj;

    [SerializeField] private string _text;
    [SerializeField] private Sprite _itemImg;
    [SerializeField] private List<CurrencyRewardData> _rewards;


    private new void Start()
    {
        base.Start();
        _textObj.text = _text;
        _itemImgObj.sprite = _itemImg;
    }

    public override void OnClick()
    {
        // обработка покупки
        foreach (var item in _rewards)
        {
            CurrencyManager.Instance.AddCurrency(item.CurrencyType, item.Amount);
        }
    }


}
