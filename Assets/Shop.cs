using System;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject _gemsShop;
    [SerializeField] private GameObject _customShop;


    private static Shop instance;
    public static Shop Instance {  get { return instance; } }

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
}
