using UnityEngine;

[CreateAssetMenu(fileName = "New Color Data", menuName = "Customizer Color")]
public class CustomizerColorData : ShopItemData
{
    [SerializeField] private string _identifier;
    [SerializeField] private Material _material;
    private ShopItemType _type = ShopItemType.Color;

    public string Identifier {  get { return _identifier; } }
    public Material Material { get { return _material; } }
    public ShopItemType Type { get { return _type; } }
}