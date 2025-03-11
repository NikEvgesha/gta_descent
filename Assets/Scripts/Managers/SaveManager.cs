using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private bool _removeSaveOnStart;
    private static SaveManager _instance;

    private List<float> _scores = new();
    private List<Tuple<int, string>> _globalScores = new();
    private List<bool> _lvls = new List<bool>();
    private Dictionary<string, bool> _colors = new();
    private int _lvlsCount = 7;


    private Dictionary<int, bool[]> _levels = new();

    private bool _initialized;
    public bool IsNewPlayer { 
        get {
            if (YG2.isSDKEnabled)
            {
                return YG2.saves.newPlayer;
            }
            return true;
        } 
        set
        {
            if (YG2.isSDKEnabled)
            {
                YG2.saves.newPlayer = value;
            }
        }
        
    }
    public static SaveManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        

    }

    private void OnEnable()
    {
        //YG2.onGetLeaderboard += LoadGlobalScores;
    }

    private void Start()
    {
        //GetLevelLeaderboards();
        if (_removeSaveOnStart)
        {
            YG2.SetDefaultSaves();
            YG2.SaveProgress();
            _removeSaveOnStart = false;
        }
        //GameLoader.Instance.LoadNextScene("Menu", true);
    }

/*    private void GetLevelLeaderboards()
    {
        for (int i = 0; i < _lvlsCount; i++)
        {
            _globalScores.Add(Tuple.Create(0, ""));
            YG2.GetLeaderboard("lvl" + (i + 1).ToString(), 1, 0);
        }
    }*/

    private void OnDisable()
    {
        YG2.SaveProgress();
    }

/*    private void LoadGlobalScores(LBData data)
    {
        int lvlId = Int32.Parse(data.technoName.Substring(3));
        if (data.players.Length > 0)
            _globalScores[lvlId - 1] = Tuple.Create(data.players[0].score, data.players[0].name);
    }*/

/*    public Tuple<int, string> GetLevelBestScore(int lvlId)
    {
        return _globalScores[lvlId - 1];
    }*/

    public float[] GetVolume()
    {
        float[] res = new float[] {0.5f, 0.5f};
        if (YG2.isSDKEnabled)
        {
            res[0] = YG2.saves.musicVolume;
            res[1] = YG2.saves.soundVolume;
        }
        return res;
    }

    public void SaveMusicVolume(float volume)
    {
        YG2.saves.musicVolume = volume;
    }

    public void SaveSoundVolume(float volume)
    {
        YG2.saves.soundVolume = volume;
    }

    public void SaveScore(float score, int lvlId)
    {
        _scores[lvlId - 1] = score;
        YG2.saves.scores[lvlId - 1] = score;
        string lbName = "lvl" + lvlId.ToString();
        YG2.SetLBTimeConvert(lbName, score);
    }

    public void LoadScores()
    {
        if (YG2.saves.scores != null && YG2.saves.scores.Count > 0)
        {
            foreach (float el in YG2.saves.scores)
            {
                _scores.Add(el);
            }
            if (YG2.saves.scores.Count < _lvlsCount)
            {
                for (int i = 0; i < _lvlsCount - YG2.saves.scores.Count; i++)
                {
                    YG2.saves.scores.Add(0);
                    _scores.Add(0);
                }
            }
        } else
        {
            for (int i = 0; i < _lvlsCount; i++) {
                YG2.saves.scores.Add(0);
                _scores.Add(0);
            }
        }
    }


    public float GetLevelScore(int lvlId)
    {
        return _scores[lvlId - 1];
    }

    public Dictionary<CurrencyType, int> LoadCurrency()
    {
        Dictionary<CurrencyType, int> res = new();
        if (YG2.isSDKEnabled)
        {
            Debug.Log("load currency from saves");
            res.Add(CurrencyType.Cups, YG2.saves.cups);
            res.Add(CurrencyType.Gems, YG2.saves.gems);
        } else
        {
            Debug.Log("load currency from defaults");
            res.Add(CurrencyType.Cups, 0);
            res.Add(CurrencyType.Gems, 0);
        }
        return res;
    }

    public void SaveCurrency(CurrencyType type, int amount)
    {
        Debug.Log("Save cuurency " + type + ": " + amount);
        switch (type)
        {
            case CurrencyType.Cups:
                YG2.saves.cups = amount;
                StartCoroutine(UpdateLeaderboard(amount));
                break;
            case CurrencyType.Gems:
                YG2.saves.gems = amount;
                break;
            default: break;
        }
    }

    private IEnumerator UpdateLeaderboard(int amount)
    {
        yield return new WaitForSeconds(1);
        YG2.SetLeaderboard("main", amount);
    }



    public void SaveLevelUnlock(int id, bool unlocked)
    {
/*        if (YG2.saves.levels.Length == 0)
        {
            YG2.saves.levels = new bool[_lvlsCount];
        }
        YG2.saves.levels[id - 1] = unlocked;*/

       if (_levels.ContainsKey(id))
        {
            _levels[id][0] = unlocked;
            YG2.saves.levels_status[id-1] = unlocked;
        }
    }

    public void SaveLevelWin(int id, bool win)
    {
        //if (YG2.saves.levels.Length == 0)
        //{
        //    YG2.saves.levels = new bool[_lvlsCount];
        //}
        //YG2.saves.levels[id - 1] = win;
        if (_levels.ContainsKey(id))
        {
            _levels[id][1] = win;
            YG2.saves.levels_win[id - 1] = win;
        }
    }



    /*    public void LoadColorsInfo(int count)
        {
            if (YG2.saves.colors_id.Length != 0 && YG2.saves.colors_status.Length != 0)
            {
                for (int i = 0; i < YG2.saves.colors_id.Length; i++)
                {
                    _colors.Add(YG2.saves.colors_id[i], YG2.saves.colors_status[i]);
                }
            }
        }*/

    public void SaveColor(CustomizerColorData data, bool purchased)
    {
        _colors[data.Identifier] = purchased;
        for (int i = 0; i < YG2.saves.colors_id.Length; i++)
        {
            if (YG2.saves.colors_id[i] == data.Identifier)
            {
                YG2.saves.colors_status[i] = purchased;
            }
        }
    }


    public Dictionary<LevelData, bool[]> GetLevelStatuses(List<LevelData> lvlsData)
    {
        _lvlsCount = lvlsData.Count;
        LoadScores();
        //GetLevelLeaderboards();

        if (YG2.saves.levels_id != null && YG2.saves.levels_id.Count > 0)
        {
            _levels = new Dictionary<int, bool[]>();
            for (int i = 0; i < YG2.saves.levels_id.Count; i++)
            {
                _levels.Add(YG2.saves.levels_id[i], new bool[] { YG2.saves.levels_status[i], YG2.saves.levels_win[i] });
            }
        }


        Dictionary<LevelData, bool[]> res = new();
        foreach (LevelData lvl in lvlsData)
        {
            if (_levels.ContainsKey(lvl.ID))
            {
                res.Add(lvl, _levels[lvl.ID]);
            } else
            {
                _levels.Add(lvl.ID, new bool[2]);
                res.Add(lvl, _levels[lvl.ID]);
                if (YG2.isSDKEnabled)
                {
                    YG2.saves.levels_id.Add(lvl.ID);
                    YG2.saves.levels_status.Add(false);
                    YG2.saves.levels_win.Add(false);
                }
            }
        }

        return res;
    }


    public Dictionary<CustomizerColorData, bool> LoadColorsStatuses(List<CustomizerColorData> colors)
    {
        /* First init */
        if (YG2.saves.colors_id == null || YG2.saves.colors_id.Length == 0)
        {
            YG2.saves.colors_id = new string[colors.Count];
            YG2.saves.colors_status = new bool[colors.Count];

            for (int i = 0; i < colors.Count; i++)
            {
                YG2.saves.colors_id[i] = colors[i].Identifier;
                YG2.saves.colors_status[i] = colors[i].IsDefault;
                _colors.Add(YG2.saves.colors_id[i], YG2.saves.colors_status[i]);
            }
        } else if (_colors.Count == 0)
        {
            /* Load from saves */

            for (int i = 0; i < YG2.saves.colors_id.Length; i++)
            {
                _colors.Add(YG2.saves.colors_id[i], YG2.saves.colors_status[i]);
            }

        }


        /* Resize for new items */

        if (_colors.Count < colors.Count) // new items added to shop
        {
            Array.Resize(ref YG2.saves.colors_id, colors.Count);
            Array.Resize(ref YG2.saves.colors_status, colors.Count);

            foreach (var item in colors)
            {
                if (!_colors.ContainsKey(item.Identifier))
                {
                    YG2.saves.colors_id[_colors.Count] = item.Identifier;
                    YG2.saves.colors_status[_colors.Count] = false;
                    _colors.Add(item.Identifier, false);
                }
            }
        } else if (_colors.Count > colors.Count) // items removed from shop
        {
            HashSet<string> existing_items = new HashSet<string>();
            foreach (var color in colors)
            {
                existing_items.Add(color.Identifier);
            }

            _colors = _colors.Where(x => existing_items.Contains(x.Key)).ToDictionary(x => x.Key, x=> x.Value);
            YG2.saves.colors_id = _colors.Keys.ToArray();
            Array.Resize(ref YG2.saves.colors_status, colors.Count);

            for (int i = 0; i < colors.Count; i++)
            {
                YG2.saves.colors_status[i] = _colors[YG2.saves.colors_id[i]];
            }

        }

        /* to ColorsData */

        Dictionary<CustomizerColorData, bool> res = new();
        foreach (var color in colors)
        {
            if (_colors.ContainsKey(color.Identifier))
            {
                res.Add(color, _colors[color.Identifier]);
            }
        }

/*        foreach (var el in res)
        {
            Debug.Log("Loaded status for " + el.Key + ": " + el.Value);
        }*/

        return res;
    }
}
