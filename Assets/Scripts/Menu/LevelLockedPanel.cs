using UnityEngine;
using UnityEngine.UI;

public class LevelLockedPanel : MonoBehaviour
{
    [SerializeField] Text _cupsPriceText;
    [SerializeField] Text _gemsPriceText;


    public void SetPrice(LevelData data)
    {
        _cupsPriceText.text = data.CupsPrice.ToString();
        _gemsPriceText.text = data.GesmsPrice.ToString();
    }

}
