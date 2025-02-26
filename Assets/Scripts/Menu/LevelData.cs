using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _ID;
    [SerializeField] private int _cupsPrice;
    [SerializeField] private int _gemsPrice;
    [SerializeField] private string _sceneName;
    [SerializeField] private List<CurrencyRewardData> _baseReward;
    [SerializeField] private List<CurrencyRewardData> _firstReward;
    [SerializeField] private Sprite _lvlImg;


    public int ID { get { return _ID; } }
    public int CupsPrice { get { return _cupsPrice; } }
    public int GemsPrice { get { return _gemsPrice; } }
    public string Scene { get { return _sceneName; } }

    public List<CurrencyRewardData> Reward { get { return _baseReward; } }
    public List<CurrencyRewardData> FirstReward { get { return _firstReward; } }

    public Sprite IMG { get { return _lvlImg; } }
}
