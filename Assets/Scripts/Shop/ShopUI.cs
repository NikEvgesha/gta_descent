using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject _gemsShop;
    [SerializeField] private GameObject _customShop;


    [SerializeField] private CurrencyShopSlot _currencyPrefab;
    [SerializeField] private CurrencyShopSlot _currencyPrefabLeft;
    [SerializeField] private CurrencyShopSlot _currencyPrefabRight;
    [SerializeField] private Transform _currencySlotParent;
    [SerializeField] private Transform _leftPackParent;
    [SerializeField] private Transform _rightPackParent;
    [SerializeField] private List<Transform> _slotRow;
    [SerializeField] private int _slotsPerRow = 3;

    private static ShopUI instance;
    public static ShopUI Instance {  get { return instance; } }

    public Action<bool> CustomShopOpen;

    private void Awake()
    {
        instance = this;
    }


    public void OpenGemsShop()
    {
        _gemsShop.SetActive(true);
    }

    public void CloseGemsShop()
    {
        _gemsShop.SetActive(false);
    }

    public void OpenCustomShop()
    {
        _customShop.SetActive(true);
        CustomShopOpen?.Invoke(true);
    }

    public void CloseCustomShop() 
    {
        _customShop.SetActive(false);
        CustomShopOpen?.Invoke(false);
        CustomizerManager.Instance.CheckActivation();
    }

    public void CloseAll()
    {
        if (_customShop.gameObject.activeInHierarchy)
        {
            CloseCustomShop();
        }
        if (_gemsShop.gameObject.activeInHierarchy)
        {
            CloseGemsShop();
        }
    }


    public void InitCurrencySpecialSlots(List<CurrencyPackData> packs, CurrencyPackData packLeft, CurrencyPackData packRight)
    {
        int rowNum = 0;
        int slotNum = 0;
        foreach (var item in packs)
        {
            CurrencyShopSlot slot = Instantiate(_currencyPrefab, _slotRow[rowNum]);
            slot.Init(item);
            slotNum++;
            if (slotNum >= _slotsPerRow)
            {
                rowNum++;
                if (rowNum>= _slotRow.Count)
                {
                    Debug.Log("Need more slots"); 
                    rowNum = _slotRow.Count-1;
                }
            }
        }

        CurrencyShopSlot left = Instantiate(_currencyPrefabLeft, _leftPackParent);
        left.Init(packLeft);

        CurrencyShopSlot right = Instantiate(_currencyPrefabRight, _rightPackParent);
        right.Init(packRight);

    }
}
