using UnityEngine;

public class LevelScore : MonoBehaviour
{

    [SerializeField] private LevelScoreUI _ui;
    private float _roundTimeStart;
    private float _roundTime;
    private int _lvl_id;
    private float _playerBestScore;
    private float _globalBestScore;

    public float CurrentTime { get { return _roundTime; } private set { } }

    private void OnEnable()
    {
        GameManager.Instance.LevelWin += OnWin;
    }

    void Start()
    {
        _roundTimeStart = Time.time;
        _lvl_id = GameManager.Instance.GetCurrentLevelData().ID;
        _playerBestScore = SaveManager.Instance.GetLevelScore(_lvl_id);
        UpdateRecord(_playerBestScore);
    }

    private void FixedUpdate()
    {
        UpdateTime();
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
