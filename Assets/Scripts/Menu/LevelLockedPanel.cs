using UnityEngine;
using UnityEngine.UI;

public class LevelLockedPanel : MonoBehaviour
{
    [SerializeField] private Text _cupsPriceText;
    [SerializeField] private Text _gemsPriceText;
    [SerializeField] private Button _cupButtonInactive;
    

    public void SetPrice(LevelData data)
    {
        _cupsPriceText.text = data.CupsPrice.ToString();
        _gemsPriceText.text = data.GemsPrice.ToString();
    }

    public void UnlockCupsButton()
    {
        _cupButtonInactive.interactable = true;
    }

}
