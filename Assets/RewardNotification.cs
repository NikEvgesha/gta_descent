using UnityEngine;
using UnityEngine.UI;

public class RewardNotification : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Image _img;

    private int _reward;

    public void SetRewardText(int reward, CurrencyType type)
    {
        _img.sprite = CurrencyManager.Instance.GetCurrencyIcon(type);
        _reward = reward;
        _text.text = "+" + reward.ToString();
    }

    private void Update()
    {
        if (!_text.isActiveAndEnabled)
        {
            
            Destroy(gameObject);
        }
    }
}
