using UnityEngine;
using UnityEngine.UI;

public class RewardText : MonoBehaviour
{
    [SerializeField] private Text _text;

    private int _reward;

    public void SetRewardText(int reward)
    {
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
