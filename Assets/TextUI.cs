using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    [SerializeField] private Text _text;

    public void Set(string text)
    {
        _text.text = text;
    }
}
