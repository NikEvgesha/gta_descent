using System;
using System.Collections.Generic;
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
    [SerializeField] private DynamicGridSpawner _colorSlotParent;

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
            Dictionary<string, string> keyValue = new Dictionary<string, string> 
            {
                {"Color",colorData.Identifier}
            };
            AnalyticsManager.Instance.LogEvent(EventName.buyColor.ToString(), keyValue);
        } else
        {
            slot.SetPurchaseStatus(false);
        }
            
    }


    public void TryBuy(CurrencyPackData packData)
    {
        if (packData.AdsReward)
        {
            AdsManager.Instance.ShowRewardedAd(packData.AdsRewardName.ToString(), (success) =>
            {
                if (success)
                {
                    AddReward(packData);
                    Debug.Log("Player received reward!");
                }
                else
                {
                    Debug.Log("Rewarded ad failed or was closed");
                }
            });
            return;
        }

        // Payment processing
        if (packData.PurchaseReward)
        {
            PurchasesManager.Instance.BuyPurchase(packData.PurchaseRewardName.ToString(), (success) =>
            {
                if (success)
                {
                    AddReward(packData);
                    Debug.Log("Purchase completed!");
                    // Дай игроку награду, например, 50 монет
                }
                else
                {
                    Debug.Log("Purchase failed!");
                }
            });
            return;
        }
        if (CurrencyManager.Instance.RemoveCurrency(packData.CurrencyType, packData.Price))
        {
            AddReward(packData);
        }

    }
    private void AddReward(CurrencyPackData packData)
    {
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
        Dictionary<string, string> keyValue = new Dictionary<string, string>
            {
                {"Item",packData.Description}
            };
        AnalyticsManager.Instance.LogEvent(EventName.buyInShop.ToString(), keyValue);
    }


    // move to UI ?

    private void InitSlots()
    {
        _shopUI.InitCurrencySpecialSlots(_currencyPacks, _specialPack_left, _specialPack_right);



        foreach (var item in _carColors) {

            ColorShopSlot slot = _colorSlotParent.SpawnObject<ColorShopSlot>(_colorSlotPrefab.gameObject);
            //ColorShopSlot slot = Instantiate(_colorSlotPrefab, _colorSlotParent);
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
