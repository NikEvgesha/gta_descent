using System;
using UnityEngine;
using YG;
using YG.Utils.LB;

public class LevelScore : MonoBehaviour
{

    [SerializeField] private LevelScoreUI _ui;
    [SerializeField] private GameObject _globalBest;
    private float _roundTimeStart;
    private float _roundTime;
    private int _lvl_id;
    private float _playerBestScore;
    private Tuple<int, string> _globalBestScore;
    private string _bestPlayerName;
    private int _bestPlayerScore;

    public float CurrentTime { get { return _roundTime; } private set { } }

    private void OnEnable()
    {
        GameManager.Instance.LevelWin += OnWin;
        YG2.onGetLeaderboard += SetGlobalRecord;
        LocalizationManager.Instance.OnLanguageChanged += onLanguageChange;
    }

    private void OnDisable()
    {
        GameManager.Instance.LevelWin -= OnWin;
        LocalizationManager.Instance.OnLanguageChanged -= onLanguageChange;
    }

    void Start()
    {
        _roundTimeStart = Time.time;
        _lvl_id = GameManager.Instance.GetCurrentLevelData().ID;
        _playerBestScore = SaveManager.Instance.GetLevelScore(_lvl_id);
        YG2.GetLeaderboard("lvl" + (_lvl_id).ToString(), 1, 0);
        //_globalBestScore = SaveManager.Instance.GetLevelBestScore(_lvl_id);
        UpdateRecord(_playerBestScore);
    }

    private void FixedUpdate()
    {
        UpdateTime();
    }

    private void onLanguageChange(string lang)
    {
        _globalBestScore = Tuple.Create(_bestPlayerScore, LBMethods.AnonymousName(_bestPlayerName));
        _ui.SetGlobalRecord(_globalBestScore);
    }


    private void SetGlobalRecord(LBData data)
    {
        if (data.technoName != "lvl" + _lvl_id)
            return;
        if (data.players.Length > 0) {
            _globalBestScore = Tuple.Create(data.players[0].score, LBMethods.AnonymousName(data.players[0].name));
            _bestPlayerName = data.players[0].name;
            _bestPlayerScore = data.players[0].score;
            _ui.SetGlobalRecord(_globalBestScore);
        } else
        {
            _globalBest.SetActive(false);
        }
    }

    private void UpdateTime()
    {
        _roundTime = Time.time - _roundTimeStart;
        _ui.SetTime(_roundTime);
    }


    private void UpdateRecord(float time)
    {
        _ui.SetRecord(time);
    }

    private void OnWin()
    {
        _roundTime = Time.time - _roundTimeStart;
        if (_roundTime < _playerBestScore || float.Equals(_playerBestScore, 0f))
        {
            SaveManager.Instance.SaveScore(_roundTime, _lvl_id);
            UpdateRecord(_roundTime);
            _playerBestScore = _roundTime;
            YG2.GetLeaderboard("lvl" + (_lvl_id).ToString(), 1, 0);
            // обновить глобальный рекорд если нужно
        }
        _roundTimeStart = Time.time;
        UpdateTime();
    }

/*    private void OnRespawn()
    {
        _roundTimeStart = Time.time;
        UpdateTime();
    }*/
}
