using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private bool _removeSaveOnStart;
    private static SaveManager _instance;

    private List<float> _scores = new List<float>();
    private List<bool> _lvls = new List<bool>();
    private List<bool> _colors = new List<bool>();
    private int _topScoreCounts = 5;
    private int _lvlsCount = 5;

    public int TopScoreCounts { get { return _topScoreCounts; } }
    public static SaveManager Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this; 
    }

    private void Start()
    {
        if (_removeSaveOnStart)
        {
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
        } else if (YG2.isSDKEnabled == true)
        {
            LoadColors();
            LoadLevels();
            Debug.Log("LOADED");
        }
    }

    private void OnDisable()
    {
        YG2.SaveProgress();
    }

    /*public void SaveScore(float score)
    {

        if (YG2.saves.scores == null)
        {
            YG2.saves.scores = new float[_topScoreCounts];
        }

        //long l_score = (long)score * 1000;
        _scores.Add(score);
        _scores.Sort();
        if (_scores.Count > _topScoreCounts)
            _scores.RemoveAt(_scores.Count - 1);
        int i = 0;
        foreach (float el in _scores)
        {
            YG2.saves.scores[i] = el;
            //Debug.Log("YG save: "+YandexGame.savesData.scores[i]);
            i++;
        }
        //Debug.Log("score: " + score);
        if (Mathf.Abs(score - _scores[0]) <= 1e-06)
        {
            //Debug.Log("score to LB: " + score);
            //YG2.SetLeaderboard("allTime", score);
            //YG2.SetLeaderboard("monthTime", score);
        }

        YG2.SaveProgress();
    }

    public void LoadScores()
    {
        if (YG2.saves.scores != null)
        {
            foreach (float el in YG2.saves.scores)
            {
                if (el != 0)
                {
                    _scores.Add(el);
                }
            }
        }
    }*/

    // TODO: переделать
    public void LoadLevels()
    {
        if (YG2.saves.levels.Length == 0)
        {
            YG2.saves.levels = new bool[_lvlsCount];
        }
    }

    public void SaveLevel(int id, bool unlocked)
    {
        if (YG2.saves.levels.Length == 0)
        {
            YG2.saves.levels = new bool[_lvlsCount];
        }
        YG2.saves.levels[id - 1] = unlocked;
    }


    public void LoadColors()
    {
        
    }

    public void SaveColor(CustomizerColorData data, bool purchased)
    {
        
    }


    public List<bool> GetLevelStatuses()
    {
        return _lvls;
    }

    public List<bool> GetColorsStatuses()
    {
        return _colors;
    }
}
