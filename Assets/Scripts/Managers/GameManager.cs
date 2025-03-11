using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    /* static */
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private LevelData _firstLevel;

    /* Events */
    public Action<bool> LevelInProgress;
    public Action LevelWin;
    //public Action LevelStart;
    //public Action LevelExit;

    private LevelData _currentLevel;
    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("delete duplicate GameMAnager");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        LevelWin += LevelWinEvent;
    }

    private void Start()
    {
        Debug.Log("Player name: " + YG2.player.name);
        if (SaveManager.Instance.IsNewPlayer)
        {
            StartLevel(_firstLevel);
            SaveManager.Instance.IsNewPlayer = false;
        } else
        {
            GameLoader.Instance.LoadNextScene("Menu", true);
        }
        
    }

    private void OnEnable()
    {
        YG2.onFocusWindowGame += OnFocusWindowGame;
    }
    private void OnDisable()
    {
        YG2.onFocusWindowGame -= OnFocusWindowGame;
    }

    private void OnFocusWindowGame(bool _inFocus)
    {
        SetPause(!_inFocus);
    }

    public void SetPause(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    public void StartLevel(LevelData data)
    {
        _currentLevel = data;
        GameLoader.Instance.LoadNextScene(data.Scene, true);
        LevelInProgress?.Invoke(true);
    }


    public void ExitLevel()
    {
        _currentLevel = null;
        GameLoader.Instance.LoadNextScene("Menu", true);
        LevelInProgress?.Invoke(false);
        AnalyticsManager.Instance.LogEvent(EventName.home.ToString());
    }

    public LevelData GetCurrentLevelData()
    {
        return _currentLevel;
    }
    private void LevelWinEvent()
    {
        Dictionary<string,string> key = new Dictionary<string,string>
        {
            { "Scene:", _currentLevel.Scene }
        };
        AnalyticsManager.Instance.LogEvent(EventName.levelEnd.ToString(), key);
    }
}
