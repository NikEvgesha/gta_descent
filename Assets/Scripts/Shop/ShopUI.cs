using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject _gemsShop;
    [SerializeField] private GameObject _customShop;


    [SerializeField] private CurrencyShopSlot _currencyPrefab;
    [SerializeField] private CurrencyShopSlot _currencyPrefabLeft;
    [SerializeField] private CurrencyShopSlot _currencyPrefabRight;
    [SerializeField] private DynamicGridSpawner _currencySlotParent;
    [SerializeField] private Transform _leftPackParent;
    [SerializeField] private Transform _rightPackParent;
    [SerializeField] private List<Transform> _slotRow;
    //[SerializeField] private int _slotsPerRow = 3;

    private static ShopUI _instance;
    public static ShopUI Instance {  get { return _instance; } }

    public Action<bool> CustomShopOpen;

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
            CurrencyShopSlot slot = _currencySlotParent.SpawnObject<CurrencyShopSlot>(_currencyPrefab.gameObject);
            //CurrencyShopSlot slot = Instantiate(_currencyPrefab, _slotRow[rowNum]);
            slot.Init(item);
            if (item.PurchaseReward)
                StartCoroutine(DownloadImage(PurchasesManager.Instance.GetPurchaseData(item.PurchaseRewardName.ToString()).CurrencyImageURL, slot));
        }

        CurrencyShopSlot left = Instantiate(_currencyPrefabLeft, _leftPackParent);
        left.Init(packLeft);

        if (packLeft.PurchaseReward)
            StartCoroutine(DownloadImage(PurchasesManager.Instance.GetPurchaseData(packLeft.PurchaseRewardName.ToString()).CurrencyImageURL, left));

        CurrencyShopSlot right = Instantiate(_currencyPrefabRight, _rightPackParent);
        right.Init(packRight);

        if (packRight.PurchaseReward)
            StartCoroutine(DownloadImage(PurchasesManager.Instance.GetPurchaseData(packRight.PurchaseRewardName.ToString()).CurrencyImageURL, right));

    }

    IEnumerator DownloadImage(string imageUrl, CurrencyShopSlot slot)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            slot.InitImage(sprite);
        }
        else
        {
            Debug.LogError("Ошибка загрузки: " + request.error);
        }
    }
}
