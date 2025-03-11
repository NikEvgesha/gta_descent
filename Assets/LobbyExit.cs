using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameMenu.Instance.OpenExitWindow();
    }
}
