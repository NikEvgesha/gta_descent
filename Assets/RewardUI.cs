using System.Collections.Generic;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private RewardNotification _rewardPrefab;
    //[SerializeField] private RewardNotification _gemsRewardPrefab;

    [SerializeField] private Transform _cupsPoint;
    [SerializeField] private Transform _gemsPoint;

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
        List<CurrencyRewardData> rewards = _currentLevel.Reward;
        if (_currentLevel)
        {
            foreach (var item in rewards)
            {
                switch (item.CurrencyType)
                {
                    case CurrencyType.Cups:
                        {
                            RewardNotification reward = Instantiate(_rewardPrefab, _cupsPoint);
                            reward.SetRewardText(item.Amount, item.CurrencyType);
                            break;
                        }
                    case CurrencyType.Gems:
                        {
                            RewardNotification reward = Instantiate(_rewardPrefab, _gemsPoint);
                            reward.SetRewardText(item.Amount, item.CurrencyType);
                            break;
                        }
                }
                CurrencyManager.Instance.AddCurrency(item.CurrencyType, item.Amount);
            }
        }
    }
}
