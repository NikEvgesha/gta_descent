using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private Sprite _gemsIcon;
    [SerializeField] private Sprite _cupsIcon;
    [SerializeField] private Sprite _yanIcon;
    private static CurrencyManager _instance;

    private Dictionary<CurrencyType, int> _balance;

    private Dictionary<CurrencyType, Sprite> _currencyIcons;

    public int Gems { get {return _balance[CurrencyType.Gems]; } }
    public int Cups { get { return _balance[CurrencyType.Cups]; } }

    public Action<int> GemsChanged;
    public Action<int> CupsChanged;
    public static CurrencyManager Instance {  get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Debug.Log("Currency Manager Awake");
            _currencyIcons = new Dictionary<CurrencyType, Sprite> {
            {CurrencyType.Gems, _gemsIcon},
            {CurrencyType.Cups, _cupsIcon},
            {CurrencyType.Real, _yanIcon},
        };
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        Debug.Log("Currency Manager start");
        _balance = SaveManager.Instance.LoadCurrency();
        Debug.Log("Balance after save: " + _balance[CurrencyType.Cups] + " cups; " + _balance[CurrencyType.Gems] + " gems");
        CupsChanged?.Invoke(_balance[CurrencyType.Cups]);
        GemsChanged?.Invoke(_balance[CurrencyType.Gems]);
    }

/*    private void Load()
    {
        _cupsAmount = YG2.saves.cups;
        _gemsAmount = YG2.saves.gems;
    }*/


    public void AddCups(int amount)
    {
        Debug.Log("Add cups: " + amount);
        _balance[CurrencyType.Cups] += amount;
        CupsChanged?.Invoke(_balance[CurrencyType.Cups]);
        SaveManager.Instance.SaveCurrency(CurrencyType.Cups, _balance[CurrencyType.Cups]);
    }

    public bool RemoveCups(int amount)
    {
        if (_balance[CurrencyType.Cups] < amount)
        {
            return false;
        }
        _balance[CurrencyType.Cups] -= amount;
        CupsChanged?.Invoke(_balance[CurrencyType.Cups]);
        SaveManager.Instance.SaveCurrency(CurrencyType.Cups, _balance[CurrencyType.Cups]);
        return true;
    }

/*    public bool CheckCups(int amount)
    {
        return _cupsAmount >= amount;
    }*/

    public void AddGems(int amount)
    {
        _balance[CurrencyType.Gems] += amount;
        GemsChanged?.Invoke(_balance[CurrencyType.Gems]);
        SaveManager.Instance.SaveCurrency(CurrencyType.Gems, _balance[CurrencyType.Gems]);
    }

    public bool RemoveGems(int amount)
    {
        if (_balance[CurrencyType.Gems] < amount)
        {
            ShopUI.Instance.OpenGemsShop();
            return false;
        }
        _balance[CurrencyType.Gems] -= amount;
        GemsChanged?.Invoke(_balance[CurrencyType.Gems]);
        SaveManager.Instance.SaveCurrency(CurrencyType.Gems, _balance[CurrencyType.Gems]);
        return true;
    }

    public Sprite GetCurrencyIcon(CurrencyType type)
    {
        return _currencyIcons[type];
    }


    public void AddCurrency(CurrencyType type, int amount)
    {
        switch (type) {
            case CurrencyType.Cups:
                {
                    AddCups(amount);
                    break;
                }
            case CurrencyType.Gems:
                {
                    AddGems(amount);
                    break;
                }
            default: break;
        }
    }

    public bool RemoveCurrency(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.Cups:
                return RemoveCups(amount);
            case CurrencyType.Gems:
                return RemoveGems(amount);
            case CurrencyType.Real:
                break;
            default: break;
        }
        return false;
    }
}
