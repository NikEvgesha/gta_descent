using UnityEngine;

public class ShopItemData : ScriptableObject
{
    [SerializeField] protected int _price;
    [SerializeField] protected CurrencyType _currencyType;
    [SerializeField] private Sprite _itemImg;

    public CurrencyType CurrencyType { get { return _currencyType; } }
    public int Price { get { return _price; } }

    public Sprite ItemIMG { get { return _itemImg; } }
}