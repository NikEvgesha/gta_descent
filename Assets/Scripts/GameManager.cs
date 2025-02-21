using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* static */
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    /* Events */
    public Action<bool> LevelInProgress;
    //public Action LevelStart;
    //public Action LevelExit;


    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
