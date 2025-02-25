using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject _exitWindow;



    public void OnClick()
    {
        _exitWindow.SetActive(true);
        //GameManager.Instance.SetPause(true);
    }


    public void ExitLevel()
    {
        GameManager.Instance.ExitLevel();
    }

    public void CancelExit()
    {
        _exitWindow.SetActive(false);
    }
}
