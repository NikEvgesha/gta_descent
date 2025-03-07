using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CurrencyRewardData
{
    public int Amount;
    public CurrencyType CurrencyType;
}

[CreateAssetMenu(fileName = "CurrencyPackData", menuName = "Currency Pack")]
public class CurrencyPackData : ShopItemData
{
    [SerializeField] private string _text;
    [SerializeField] private List<CurrencyRewardData> _currencyReward;
    [SerializeField] private List<CustomizerColorData> _itemReward;
    [SerializeField] private bool _adsReward = false;
    [SerializeField] private AdsReward _adsRewardName;
    [SerializeField] private bool _purchaseReward = false;
    [SerializeField] private PurchaseReward _purchaseRewardName;

    public string Description { get { return _text; } }
    public List<CurrencyRewardData> CurrencyRewards { get { return _currencyReward; } }
    public List<CustomizerColorData> ItemRewards { get { return _itemReward; } }
    public bool AdsReward { get { return _adsReward; } }
    public AdsReward AdsRewardName { get { return _adsRewardName; } }
    public bool PurchaseReward { get { return _purchaseReward; } }
    public PurchaseReward PurchaseRewardName { get { return _purchaseRewardName; } }

}