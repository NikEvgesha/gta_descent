using UnityEngine;

public class ColorShopSlot : ShopSlot
{
    [SerializeField] private CustomizerColorData _colorData;
    [SerializeField] private GameObject _buyInfo;

    private bool _purchased;
    private bool _activated;

/*    private new void Start()
    {
        base.Start();
        _img = GetComponent<Image>();
        _img.sprite = _colorData.ItemIMG;
    }*/

    public void Init(CustomizerColorData data, bool purchased)
    {
        base.Init(data);
        _colorData = data;
        _itemImgObj.sprite = _colorData.ItemIMG;
        _purchased = purchased;
        if (_purchased)
        {
            _buyInfo.SetActive(false);
        }
    }


    public void OnPreviewClick()
    {
        CustomizerManager.Instance.Activate(_colorData, _purchased);
        _activated = true;
    }

    public override void OnClick()
    {
        if (!_purchased)
        {
            Shop.Instance.TryBuy(_colorData);
        }
    }

}
