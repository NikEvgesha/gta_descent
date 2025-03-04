using UnityEngine;
using YG;

public class CarInput : MonoBehaviour
{
    [SerializeField] private bool _useTouchControls = false;
    [SerializeField] private UIButton _uIButton;

    private PrometeoTouchInput _throttlePTI, _reversePTI, _turnRightPTI, _turnLeftPTI, _handbrakePTI, _spawnPTI;
    public bool ThrottlePressed { get; private set; }
    public bool ReversePressed { get; private set; }
    public bool TurnLeftPressed { get; private set; }
    public bool TurnRightPressed { get; private set; }
    public bool HandbrakePressed { get; private set; }
    public bool SpawnPressed { get; private set; }

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public bool IsCursorVisible { get; private set; }

    private void Awake()
    {
        UIControls uiControls = FindAnyObjectByType<UIControls>();
        if (uiControls != null)
        {
#if !UNITY_EDITOR
            _useTouchControls = !YG2.envir.isDesktop;
            uiControls.UseMobileSetup(_useTouchControls);
#else
            uiControls.UseMobileSetup(_useTouchControls);
#endif
        }
        _uIButton = uiControls.GetButton();
        if (ValidateTouchControls())
        {
            _throttlePTI = _uIButton._throttleButton;
            _reversePTI = _uIButton._reverseButton;
            _turnLeftPTI = _uIButton._turnLeftButton;
            _turnRightPTI = _uIButton._turnRightButton;
            _handbrakePTI = _uIButton._handbrakeButton;
            _spawnPTI = _uIButton._spawnButton;
        }

        IsCursorVisible = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        IsCursorVisible = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (_useTouchControls)
        {
            ThrottlePressed = _throttlePTI.buttonPressed;
            ReversePressed = _reversePTI.buttonPressed;
            TurnLeftPressed = _turnLeftPTI.buttonPressed;
            TurnRightPressed = _turnRightPTI.buttonPressed;
            HandbrakePressed = _handbrakePTI.buttonPressed;
            SpawnPressed = _spawnPTI.buttonPressed;
        }
        else
        {
            ThrottlePressed = Input.GetKey(KeyCode.W);
            ReversePressed = Input.GetKey(KeyCode.S);
            TurnLeftPressed = Input.GetKey(KeyCode.A);
            TurnRightPressed = Input.GetKey(KeyCode.D);
            HandbrakePressed = Input.GetKey(KeyCode.Space);
            SpawnPressed = Input.GetKey(KeyCode.F);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            IsCursorVisible = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(1))
        {
            IsCursorVisible = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (!IsCursorVisible)
        {
            MouseX = Input.GetAxis("Mouse X");
            MouseY = Input.GetAxis("Mouse Y");
        }
        else
        {
            MouseX = 0f;
            MouseY = 0f;
        }
    }

    private bool ValidateTouchControls()
    {
        if (_uIButton._throttleButton == null || _uIButton._reverseButton == null || _uIButton._turnRightButton == null ||
            _uIButton._turnLeftButton == null || _uIButton._handbrakeButton == null || _uIButton._spawnButton == null)
        {
            Debug.LogWarning("Touch controls are not fully set up. Assign all buttons in the inspector.");
            return false;
        }
        return true;
    }
    public RectTransform GetCameraArea()
    {
        return _uIButton.CameraArea;
    }
}