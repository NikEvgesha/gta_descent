using UnityEngine;

public class CarCustomizer : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;

    private void Start()
    {
        CustomizerManager.Instance.ColorSet += SetColor;
        SetColor(CustomizerManager.Instance.GetActiveColor());
    }


    private void SetColor(CustomizerColorData data)
    {
        _mesh.material = data.Material;
    }

}
