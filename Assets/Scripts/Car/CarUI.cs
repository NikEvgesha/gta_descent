using UnityEngine;
using UnityEngine.UI;

public class CarUI : MonoBehaviour
{
    [SerializeField] private bool _useUI = false;
    [SerializeField] private Text _carSpeedText;

    private CarPhysics _physics;

    private void Awake()
    {
        _physics = GetComponent<CarPhysics>();
    }

    public void UpdateUI()
    {
        if (!_useUI || _carSpeedText == null) return;
        _carSpeedText.text = Mathf.RoundToInt(Mathf.Abs(_physics.CarSpeed)).ToString();
    }
}