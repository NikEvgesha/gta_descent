using System;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    /* static */
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    /* Events */
    public Action<bool> LevelInProgress;
    public Action LevelWin;
    //public Action LevelStart;
    //public Action LevelExit;


    private LevelData _currentLevel;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SaveManager.Instance.IsNewPlayer)
        {
            StartLevel(LevelsManager.Instance.GetFirstLevel());
            SaveManager.Instance.IsNewPlayer = false;
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
    }

    public LevelData GetCurrentLevelData()
    {
        return _currentLevel;
    }
}
