using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private static Shop _instance;
    [Header("Items")]
    [SerializeField] List<CustomizerColorData> _carColors;
    [SerializeField] List<CurrencyPackData> _currencyPacks;
    [SerializeField] CurrencyPackData _specialPack_left;
    [SerializeField] CurrencyPackData _specialPack_right;


    [Header("Slot Prefabs")]
    [SerializeField] private ColorShopSlot _colorSlotPrefab;
    [SerializeField] private Transform _colorSlotParent;
/*    [SerializeField] private CurrencyShopSlot _currencyPrefab;
    [SerializeField] private Transform _currencySlotParent;*/

    [Header("Elements")]
    [SerializeField] private BuyButton _buyButton;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private ShopUI _shopUI;

    private Dictionary<CustomizerColorData, bool> _colorStatuses = new Dictionary<CustomizerColorData, bool>();
    private Dictionary<CustomizerColorData, ColorShopSlot> _shopSlots = new();

    public Action ItemPurchased;

    public static Shop Instance {  get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //_carColors.ForEach(x => _colorStatuses.Add(x, false));
        _colorStatuses = SaveManager.Instance.LoadColorsStatuses(_carColors);
        InitSlots();
    }


    public void TryBuy(CustomizerColorData colorData, ColorShopSlot slot)
    {
        if (CurrencyManager.Instance.RemoveCurrency(colorData.CurrencyType, colorData.Price))
        {
            _colorStatuses[colorData] = true;
            SaveManager.Instance.SaveColor(colorData, true);
            CustomizerManager.Instance.Activate(colorData, true);
            slot.SetPurchaseStatus(true);
        } else
        {
            slot.SetPurchaseStatus(false);
        }
            
    }


    public void TryBuy(CurrencyPackData packData)
    {
        // Payment processing
        foreach (var item in packData.CurrencyRewards)
        {
            CurrencyManager.Instance.AddCurrency(item.CurrencyType, item.Amount);
        }
        foreach (var item in packData.ItemRewards)
        {
            _colorStatuses[item] = true;
            SaveManager.Instance.SaveColor(item, true);
            _shopSlots[item].SetPurchaseStatus(true);
        }
    }


    // move to UI ?

    private void InitSlots()
    {
        _shopUI.InitCurrencySpecialSlots(_currencyPacks, _specialPack_left, _specialPack_right);



        foreach (var item in _carColors) {
            ColorShopSlot slot = Instantiate(_colorSlotPrefab, _colorSlotParent);
            slot.Init(item, _colorStatuses[item], _buyButton, _toggleGroup);
            _shopSlots.Add(item, slot);
        }

/*        foreach (var item in _currencyPacks)
        {
            CurrencyShopSlot slot = Instantiate(_currencyPrefab, _currencySlotParent);
            slot.Init(item);
        }*/
    }
}
