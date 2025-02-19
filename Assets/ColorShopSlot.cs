using UnityEngine;
using UnityEngine.UI;

public class ColorShopSlot : ShopSlot
{
    [SerializeField] private Color _color;

    private Image _img;
    private new void Start()
    {
        base.Start();
        _img = GetComponent<Image>();
        _img.color = _color;
    }

    public override void OnClick()
    {
        Debug.Log("color buy");
    }
}
