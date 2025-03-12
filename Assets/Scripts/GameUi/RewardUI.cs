using System.Collections.Generic;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private RewardNotification _rewardPrefab;
    //[SerializeField] private RewardNotification _gemsRewardPrefab;

    [SerializeField] private Transform _cupsPoint;
    [SerializeField] private Transform _gemsPoint;
    [SerializeField] private ShowReward _showReward;
    private List<CurrencyRewardData> _rewards;

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
        //AddReward();
        _currentLevel = GameManager.Instance.GetCurrentLevelData();
        if (_currentLevel)
        {
            _showReward.gameObject.SetActive(true);
            bool firstWin = !(LevelsManager.Instance.CheckLevelWin(_currentLevel.ID));
            
            if (firstWin)
            {
                _rewards = _currentLevel.FirstReward;
                LevelsManager.Instance.UpdateFirstWin(_currentLevel.ID);
            }
            else
            {
                _rewards = _currentLevel.Reward;
            }
            _showReward.SetRevard(firstWin, _rewards);
        }
    }
    public void AddReward(bool x2Reward = false)
    {
        AdsManager.Instance.ShowInterstitialAd();
        _showReward.gameObject.SetActive(false);
        foreach (var item in _rewards)
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
