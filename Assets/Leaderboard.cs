using UnityEngine;
using YG;
using YG.Utils.LB;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderboard;
    [SerializeField] private GameObject _uiPanel;

    private LBData _data;
    private void OnEnable()
    {
        //YG2.onGetLeaderboard += RecieveData;
    }
    private void Start()
    {
        //YG2.GetLeaderboard("main", 5, 1);
    }

/*    private void RecieveData(LBData data)
    {
        if (data.technoName != "main")
            return;
        _data = data;
    }*/

    public void Open()
    {
        _uiPanel.gameObject.SetActive(true);
        _leaderboard.UpdateLB();
    }
}
