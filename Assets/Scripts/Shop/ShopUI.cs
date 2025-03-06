using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject _gemsShop;
    [SerializeField] private GameObject _customShop;


    [SerializeField] private CurrencyShopSlot _currencyPrefab;
    [SerializeField] private Transform _currencySlotParent;
    [SerializeField] private Transform _leftPackParent;
    [SerializeField] private Transform _rightPackParent;

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
        foreach (var item in packs)
        {
            CurrencyShopSlot slot = Instantiate(_currencyPrefab, _currencySlotParent);
            slot.Init(item);
        }

        CurrencyShopSlot left = Instantiate(_currencyPrefab, _leftPackParent);
        left.Init(packLeft);

        CurrencyShopSlot right = Instantiate(_currencyPrefab, _rightPackParent);
        right.Init(packRight);

    }
}
