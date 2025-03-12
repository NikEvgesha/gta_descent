using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject _exitWindow;

    private static GameMenu _instance;
    public static GameMenu Instance { get { return _instance; } }

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

    public void OpenExitWindow()
    {
        GameManager.Instance.SetPause(true);
        _exitWindow.SetActive(true);
        //GameManager.Instance.SetPause(true);
    }


    public void ExitLevel()
    {
        GameManager.Instance.SetPause(false);
        GameManager.Instance.ExitLevel();
    }

    public void CancelExit()
    {
        GameManager.Instance.SetPause(false);
        _exitWindow.SetActive(false);
    }
}
