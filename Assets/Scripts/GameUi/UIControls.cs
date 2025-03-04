using System;
using UnityEngine;
using YG;
[Serializable]
public struct UIButton
{
    public PrometeoTouchInput _throttleButton;
    public PrometeoTouchInput _reverseButton;
    public PrometeoTouchInput _turnRightButton;
    public PrometeoTouchInput _turnLeftButton;
    public PrometeoTouchInput _handbrakeButton;
    public PrometeoTouchInput _spawnButton;
    public RectTransform CameraArea;
}
public class UIControls : MonoBehaviour
{
    [SerializeField]
    private GameObject _mobileUI;
    [SerializeField]
    private GameObject _desktopUI;
    [SerializeField] private UIButton _uIButton;
    private void Awake()
    {
       if (YG2.envir.isDesktop)
        {
            //OnDesktopUI();
        }
        else
        {
            OnMobileUI();
        }
    }
    public void UseMobileSetup(bool isMobile)
    {
        if (isMobile)
        {
            OnMobileUI();
        }
        else
        {
            OnDesktopUI();
        }
    }
    private void OnMobileUI()
    {
        _mobileUI.SetActive(true);
        _desktopUI.SetActive(false);

    }
    private void OnDesktopUI()
    {
        _mobileUI.SetActive(false);
        _desktopUI.SetActive(true);
    }
    public UIButton GetButton()
    {
        return _uIButton;
    }
}
