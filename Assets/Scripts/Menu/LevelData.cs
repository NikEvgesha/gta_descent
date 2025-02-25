using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _ID;
    [SerializeField] private int _cupsPrice;
    [SerializeField] private int _gemsPrice;
    [SerializeField] private string _sceneName;
    [SerializeField] private int _baseReward;


    public int ID { get { return _ID; } }
    public int CupsPrice { get { return _cupsPrice; } }
    public int GesmsPrice { get { return _gemsPrice; } }
    public string Scene { get { return _sceneName; } }

    public int Reward { get { return _baseReward; } }
}
