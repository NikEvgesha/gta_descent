using UnityEngine;
using YG;

public class UIControls : MonoBehaviour
{
    [SerializeField]
    private GameObject _mobileUI;
    [SerializeField]
    private GameObject _desktopUI;
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
}
