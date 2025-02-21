using UnityEngine;
using UnityEngine.UI;

public class ColorShopSlot : ShopSlot, IPurchasable
{
    [SerializeField] private CustomizerColorData _colorData;
    [SerializeField] private GameObject _activeIndicator;

    private BuyButton _buyButton;

    private bool _purchased;
    private bool _activated;
    private Toggle _toggle;

    /*    private new void Start()
        {
            base.Start();
            _img = GetComponent<Image>();
            _img.sprite = _colorData.ItemIMG;
        }*/

    public void Init(CustomizerColorData data, bool purchased, BuyButton buyButton, ToggleGroup toggleGroup)
    {
        //base.Init(data);
        _toggle = GetComponent<Toggle>();
        _toggle.group = toggleGroup;
        _buyButton = buyButton;
        _colorData = data;
        _itemImgObj.sprite = _colorData.ItemIMG;
        _purchased = purchased;
    }

    public override void OnClick()
    {
        if (_toggle.isOn)
        {
            CustomizerManager.Instance.Activate(_colorData, _purchased);
            _activated = true;
            _activeIndicator.SetActive(true);
            if (!_purchased) 
                _buyButton.ShowButton(_colorData, this);
            else
                _buyButton.HideButton();

        } else
        {
            _activated = false;
            _activeIndicator.SetActive(false);
        }

    }

    public void OnBuyButtonClick()
    {
        if (!_purchased)
        {
            Shop.Instance.TryBuy(_colorData, this);
        }
        
    }

    public void SetPurchaseStatus(bool purchased)
    {
        Debug.Log("Set status: " + purchased);
        _purchased = purchased;
        if (_activated && _purchased)
        {
            _buyButton.HideButton();
        }
    }

}
