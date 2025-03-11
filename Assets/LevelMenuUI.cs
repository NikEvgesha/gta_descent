using System.Collections.Generic;
using UnityEngine;

public class LevelMenuUI : MonoBehaviour
{
    [SerializeField] private LevelMenuItem _lvlPrefab;
    [SerializeField] private DynamicGridSpawner _lvlParent;

    private List<LevelData> _levelsData;
    private Dictionary<LevelData, bool[]> _levelsSaveInfo;
    private Dictionary<LevelData, LevelMenuItem> _lvlsSlots;

    private void OnEnable()
    {
        LevelsManager.Instance.LevelUnlock += onLevelUnlock;
    }

    private void OnDisable()
    {
        LevelsManager.Instance.LevelUnlock -= onLevelUnlock;
    }

    void Start()
    {
        InitSlots();
    }

    private void InitSlots()
    {
        _lvlsSlots = new();
        _levelsData = LevelsManager.Instance.GetLevelsData();
        foreach (var item in _levelsData)
        {
            LevelMenuItem slot = _lvlParent.SpawnObject<LevelMenuItem>(_lvlPrefab.gameObject);
            //LevelMenuItem slot = Instantiate(_lvlPrefab, _lvlParent);
            bool[] saveData = LevelsManager.Instance.GetLevelSaveInfo(item);
            slot.Init(item, saveData[0], saveData[1]);
            _lvlsSlots.Add(item, slot);
        }
    }

    private void onLevelUnlock(LevelData lvlData)
    {
        _lvlsSlots[lvlData].UnlockLevel();
    }
}
