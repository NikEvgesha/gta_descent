using System;
using UnityEngine;
using YG;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private bool _cheat;
    private static CurrencyManager _instance;
    private int _gemsAmount;
    private int _cupsAmount;

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
            return false;
        }
        _gemsAmount -= amount;
        GemsChanged?.Invoke(_gemsAmount);
        return true;
    }
}
