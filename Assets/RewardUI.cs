using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private RewardText _prefab;

    private LevelData _currentLevel;
    private void OnEnable()
    {
        GameManager.Instance.LevelWin += ShowReward;
    }

    private void OnDisable()
    {
        GameManager.Instance.LevelWin -= ShowReward;
    }



    private void ShowReward()
    {
        _currentLevel = GameManager.Instance.GetCurrentLevelData();
        if (_currentLevel)
        {
            RewardText reward = Instantiate(_prefab, transform);
            reward.SetRewardText(_currentLevel.Reward);
            CurrencyManager.Instance.AddCurrency(CurrencyType.Cups, _currentLevel.Reward);
        }
    }
}
