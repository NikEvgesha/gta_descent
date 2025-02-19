using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject _gemsShop;
    [SerializeField] private GameObject _itemsShop;


    private static Shop instance;
    public static Shop Instance {  get { return instance; } }

    private void Awake()
    {
        instance = this;
    }


    public void OpenGemsShop()
    {
        _gemsShop.SetActive(true);
    }
}
