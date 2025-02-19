using UnityEngine;

[CreateAssetMenu(fileName = "New Color Data", menuName = "Customizer Color")]
public class CustomizerColorData : ScriptableObject
{
    [SerializeField] private Material _material;
    [SerializeField] private int _index;


    public int IDX { get { return _index; } }
    public Material Material { get { return _material; } }
}