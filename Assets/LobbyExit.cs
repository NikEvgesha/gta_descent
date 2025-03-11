using UnityEngine;

public class LobbyExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<CarInput>() != null)
        {
            other.gameObject.GetComponent<CarInput>()?.ShowCursor();
            GameMenu.Instance.OpenExitWindow();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarInput>() != null)
        {
            other.gameObject.GetComponent<CarInput>()?.HideCursor();
            GameMenu.Instance.CancelExit();
        }
    }
}
