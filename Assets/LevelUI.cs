using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameMenu _menu;
    private void OnEnable()
    {
        GameManager.Instance.LevelInProgress += OnLevelSwitch;
    }

    private void OnLevelSwitch(bool inProgress)
    {
        _menu.gameObject.SetActive(inProgress);
    }
}
