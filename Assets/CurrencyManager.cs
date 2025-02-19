using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private bool _cheat;
    [SerializeField] private Sprite _gemsIcon;
    [SerializeField] private Sprite _cupsIcon;
    [SerializeField] private Sprite _yanIcon;
    private static CurrencyManager _instance;
    private int _gemsAmount;
    private int _cupsAmount;

    private Dictionary<CurrencyType, Sprite> _currencyIcons;

    public int Gems { get {return _gemsAmount; } }
    public int Cups { get { return _cupsAmount; } }

    public Action<int> GemsChanged;
    public Action<int> CupsChanged;
    public static CurrencyManager Instance {  get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _currencyIcons = new Dictionary<CurrencyType, Sprite> {
            {CurrencyType.Gems, _gemsIcon},
            {CurrencyType.Cups, _cupsIcon},
            {CurrencyType.Real, _yanIcon},
        };

        if (_cheat)
        {
            _cupsAmount = 15;
            _gemsAmount = 500;
        }
         else if (YG2.isSDKEnabled == true)
        {
            Load();
        }
        CupsChanged?.Invoke(_cupsAmount);
        GemsChanged?.Invoke(_gemsAmount);
    }

    private void Load()
    {
        _cupsAmount = YG2.saves.cups;
        _gemsAmount = YG2.saves.gems;
    }

    public void AddCups(int amount)
    {
        _cupsAmount += amount;
        CupsChanged?.Invoke(_cupsAmount);
    }

    public bool RemoveCups(int amount)
    {
        if (_cupsAmount < amount)
        {
            return false;
        }
        _cupsAmount -= amount;
        CupsChanged?.Invoke(_cupsAmount);
        return true;
    }

/*    public bool CheckCups(int amount)
    {
        return _cupsAmount >= amount;
    }*/

    public void AddGems(int amount)
    {
        _gemsAmount += amount;
        GemsChanged?.Invoke(_gemsAmount);
    }

    public bool RemoveGems(int amount)
    {
        if (_gemsAmount < amount)
        {
            // TODO: open shop
            Shop.Instance.OpenGemsShop();
            return false;
        }
        _gemsAmount -= amount;
        GemsChanged?.Invoke(_gemsAmount);
        return true;
    }

    public Sprite GetCurrencyIcon(CurrencyType type)
    {
        return _currencyIcons[type];
    }
}
