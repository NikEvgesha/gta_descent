using System;
using UnityEngine;

public class CustomizerManager : MonoBehaviour
{
    private static CustomizerManager instance;
    public static CustomizerManager Instance {  get { return instance; } }


    [SerializeField] CustomizerColorData _defaultColor;

    public Action<CustomizerColorData> ColorSet;

    
    private CustomizerColorData _currentColor;
    private bool _currentPurchased;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Activate(CustomizerColorData colorData, bool purchased)
    {
        _currentColor = colorData;
        _currentPurchased = purchased;
        ColorSet?.Invoke(colorData);
    }

    public void CheckActivation()
    {
        if (!_currentPurchased)
        {
            _currentColor = _defaultColor;
            ColorSet?.Invoke(_defaultColor);
        }
    }

    public CustomizerColorData GetActiveColor()
    {
        if (_currentColor == null)
        {
            _currentColor = _defaultColor;
        }
        return _currentColor;
    }

}
