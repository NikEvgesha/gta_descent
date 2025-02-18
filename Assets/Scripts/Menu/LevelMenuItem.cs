using UnityEngine;

public class LevelMenuItem : MonoBehaviour
{
    [SerializeField] LevelLockedPanel _lvlLockedPanel;
    [SerializeField] LevelData _levelData;
    [SerializeField] bool _unlocked = false;

    private void Start()
    {
        // load statuses from saves
        if (_unlocked)
        {
            _lvlLockedPanel.gameObject.SetActive(false);
        } else
        {
            _lvlLockedPanel.SetPrice(_levelData);
        }
    }



    
}
