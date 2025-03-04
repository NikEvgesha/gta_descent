using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public static Settings instance;
    public Action<float> ChangeMouseSensitivity;
    public Action<float> ChangeVolume;
    //public Action<bool> ControllJostick;
    //[SerializeField] private Toggle _joystickToggle;
    [SerializeField] private GameObject _UIWindow;

    //private bool _isOpen;


/*    private void OnEnable()
    {
        UIInputHandler.Instance.SettingaOpenAction += ToggleUIOpen;
        _isOpen = false;
    }

    private void OnDisable()
    {
        UIInputHandler.Instance.SettingaOpenAction -= ToggleUIOpen;
    }*/


    // �������� ������ � ����� - ������ ��������
    private Settings()
    {
        instance = this;
    }
    public void Sensitivity(float sens)
    {
        ChangeMouseSensitivity?.Invoke(sens);
    }

    public void SoundVolume(float volume)
    {
        SoundManager.Instance.SoundVolume = volume;
    }
    public void MusicVolume(float volume)
    {
        SoundManager.Instance.MusicVolume = volume;
    }

/*    private void ToggleUIOpen()
    {
        _isOpen = !_isOpen;
        _UIWindow.SetActive(_isOpen);
        if (ControlManager.Instance && ControlManager.Instance.UseCursor != _isOpen)
        {
            ControlManager.Instance.UnlockMouse();
        }
    }*/

}
