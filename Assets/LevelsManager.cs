using System.Collections.Generic;
using UnityEngine;
using YG;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] private List<LevelMenuItem> _lvlItems;

    private int _lvlCount;

    private void Start()
    {
        _lvlCount = _lvlItems.Count;
        if (YG2.saves.levels == null)
        {
            YG2.saves.levels = new bool[_lvlCount];
        }
    }


}
