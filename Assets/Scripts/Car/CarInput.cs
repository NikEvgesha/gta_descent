// CarInput.cs
using UnityEngine;

public class CarInput : MonoBehaviour
{
    [SerializeField] private bool _useTouchControls = false;
    [SerializeField] private GameObject _throttleButton;
    [SerializeField] private GameObject _reverseButton;
    [SerializeField] private GameObject _turnRightButton;
    [SerializeField] private GameObject _turnLeftButton;
    [SerializeField] private GameObject _handbrakeButton;
    [SerializeField] private GameObject _spawnButton;

    private PrometeoTouchInput _throttlePTI, _reversePTI, _turnRightPTI, _turnLeftPTI, _handbrakePTI, _spawnPTI;
    public bool ThrottlePressed { get; private set; }
    public bool ReversePressed { get; private set; }
    public bool TurnLeftPressed { get; private set; }
    public bool TurnRightPressed { get; private set; }
    public bool HandbrakePressed { get; private set; }
    public bool SpawnPressed { get; private set; }

    private void Awake()
    {
        if (_useTouchControls && ValidateTouchControls())
        {
            _throttlePTI = _throttleButton.GetComponent<PrometeoTouchInput>();
            _reversePTI = _reverseButton.GetComponent<PrometeoTouchInput>();
            _turnLeftPTI = _turnLeftButton.GetComponent<PrometeoTouchInput>();
            _turnRightPTI = _turnRightButton.GetComponent<PrometeoTouchInput>();
            _handbrakePTI = _handbrakeButton.GetComponent<PrometeoTouchInput>();
            _spawnPTI = _spawnButton.GetComponent<PrometeoTouchInput>();
        }
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

        // Добавляем отладку
        if (ThrottlePressed) Debug.Log("Throttle Pressed");
        if (ReversePressed) Debug.Log("Reverse Pressed");
    }

    private bool ValidateTouchControls()
    {
        if (_throttleButton == null || _reverseButton == null || _turnRightButton == null ||
            _turnLeftButton == null || _handbrakeButton == null || _spawnButton == null)
        {
            Debug.LogWarning("Touch controls are not fully set up. Assign all buttons in the inspector.");
            return false;
        }
        return true;
    }
}