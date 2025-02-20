using System;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private static Shop instance;
    [Header("Items")]
    [SerializeField] List<CustomizerColorData> _carColors;
    [SerializeField] List<CurrencyPackData> _currencyPacks;


    [Header("Slot Prefabs")]
    [SerializeField] private ColorShopSlot _colorSlotPrefab;
    [SerializeField] private Transform _colorSlotParent;
    [SerializeField] private CurrencyShopSlot _currencyPrefab;
    [SerializeField] private Transform _currencySlotParent;

    private Dictionary<CustomizerColorData, bool> _colorStatuses = new Dictionary<CustomizerColorData, bool>();

    public Action ItemPurchased;

    public static Shop Instance {  get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _carColors.ForEach(x => _colorStatuses.Add(x, false));
        //LoadStatuses();
        InitSlots();
    }


    public void TryBuy(CustomizerColorData colorData)
    {
        if (CurrencyManager.Instance.RemoveCurrency(colorData.CurrencyType, colorData.Price))
        {
            _colorStatuses[colorData] = true;
            SaveManager.Instance.SaveColor(colorData, true);
            CustomizerManager.Instance.Activate(colorData, true);
        }
    }


    public void TryBuy(CurrencyPackData packData)
    {
        // Payment processing
        foreach (var item in packData.Rewards)
        {
            CurrencyManager.Instance.AddCurrency(item.CurrencyType, item.Amount);
        }
    }


    // move to UI ?

    private void InitSlots()
    {
        foreach (var item in _carColors) {
            ColorShopSlot slot = Instantiate(_colorSlotPrefab, _colorSlotParent);
            slot.Init(item, _colorStatuses[item]);
        }

        foreach (var item in _currencyPacks)
        {
            CurrencyShopSlot slot = Instantiate(_currencyPrefab, _currencySlotParent);
            slot.Init(item);
        }
    }
}
