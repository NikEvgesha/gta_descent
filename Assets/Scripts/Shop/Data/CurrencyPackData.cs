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
    [SerializeField] private List<CurrencyRewardData> _rewards;

    public string Description { get { return _text; } }
    public List<CurrencyRewardData> Rewards { get { return _rewards; } }

}