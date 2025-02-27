using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelScoreUI : MonoBehaviour
{
    [SerializeField] private Text _currentTime;
    [SerializeField] private Text _bestTime;
    [SerializeField] private Text _bestGlobalTime;
    [SerializeField] private Text _bestGlobalName;
    void Start()
    {
        
    }

    public void SetTime(float time)
    {
        var timeSpan = TimeSpan.FromMilliseconds(time * 1000);
        _currentTime.text = string.Format("{0:D2}:{1:D2}.{2:000}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    public void SetRecord(float time)
    {
        var timeSpan = TimeSpan.FromMilliseconds(time * 1000);
        _bestTime.text = string.Format("{0:D2}:{1:D2}.{2:000}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    public void SetGlobalRecord(Tuple<int, string> data)
    {
        var timeSpan = TimeSpan.FromMilliseconds(data.Item1);
        _bestGlobalTime.text = string.Format("{0:D2}:{1:D2}.{2:000}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds);
        _bestGlobalName.text = data.Item2;
    }
}
