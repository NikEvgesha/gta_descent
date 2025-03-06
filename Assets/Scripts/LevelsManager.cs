using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    private static LevelsManager _instance;
    [SerializeField] private List<LevelData> _levelsData;

    [SerializeField] private LevelMenuItem _lvlPrefab;
    [SerializeField] private Transform _lvlParent;

    private Dictionary<LevelData, bool[]> _levelsSaveInfo;
    private Dictionary<LevelData, LevelMenuItem> _lvlsSlots;

    private int _lvlCount;

    public static LevelsManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _lvlsSlots = new();
        _levelsSaveInfo = SaveManager.Instance.GetLevelStatuses(_levelsData);
        CheckDefault();
        InitSlots();
    }

    public LevelData GetFirstLevel()
    {
        return _levelsData[0];
    }


    private void InitSlots()
    {
        foreach (var item in _levelsData)
        {
            LevelMenuItem slot = Instantiate(_lvlPrefab, _lvlParent);
            slot.Init(item, _levelsSaveInfo[item][0], _levelsSaveInfo[item][1]);
            _lvlsSlots.Add(item, slot);
        }
    }

    private void CheckDefault()
    {
        if (_levelsSaveInfo[_levelsData[0]][0] == false)
        {
            SaveManager.Instance.SaveLevelUnlock(0, true);
            _levelsSaveInfo[_levelsData[0]][0] = true;
        }
    }

    public void TryUnlockLevel(LevelData lvlData, CurrencyType type)
    {
        bool buy_res = false;
        switch (type)
        {
            case CurrencyType.Cups:
            {
                buy_res = CurrencyManager.Instance.RemoveCups(lvlData.CupsPrice);
                break;
            }
            case CurrencyType.Gems:
            {
                buy_res = CurrencyManager.Instance.RemoveGems(lvlData.GemsPrice);
                break;
            }
            default: break;
        }

        if (buy_res)
        {
            SaveManager.Instance.SaveLevelUnlock(lvlData.ID, true);
            _lvlsSlots[lvlData].UnlockLevel();
        }
    }


    public bool CheckLevelWin(int lvlIdx)
    {
        return _levelsSaveInfo[_levelsData[lvlIdx - 1]][1];
    }

    public bool CheckLevelUnlock(int lvlIdx)
    {
        return _levelsSaveInfo[_levelsData[lvlIdx - 1]][0];
    }

    public void UpdateFirstWin(int lvlIdx)
    {
        _levelsSaveInfo[_levelsData[lvlIdx - 1]][1] = true;
        SaveManager.Instance.SaveLevelWin(lvlIdx, true);
    } 

}
