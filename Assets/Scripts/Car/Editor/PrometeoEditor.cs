using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CarController))]
public class PrometeoEditor : Editor
{
    private CarController _controller;
    private CarPhysics _physics;
    private CarEffects _effects;
    private CarAudio _audio;
    private CarUI _ui;
    private CarInput _input;
    private CarTeleport _teleport;

    private SerializedObject _physicsSO, _effectsSO, _audioSO, _uiSO, _inputSO;

    private bool _showPhysics, _showEffects, _showAudio, _showUI, _showInput;

    private void OnEnable()
    {
        Debug.Log($"Target type: {target.GetType().Name}");

        if (target is CarController carController)
        {
            _controller = carController;
        }
        else
        {
            Debug.LogError($"Target is not a CarController! Found type: {target.GetType().Name}");
            return;
        }

        _physics = _controller.GetComponent<CarPhysics>();
        _effects = _controller.GetComponent<CarEffects>();
        _audio = _controller.GetComponent<CarAudio>();
        _ui = _controller.GetComponent<CarUI>();
        _input = _controller.GetComponent<CarInput>();
        _teleport = _controller.GetComponent<CarTeleport>();

        _physicsSO = new SerializedObject(_physics);
        _effectsSO = new SerializedObject(_effects);
        _audioSO = new SerializedObject(_audio);
        _uiSO = new SerializedObject(_ui);
        _inputSO = new SerializedObject(_input);
    }

    public override void OnInspectorGUI()
    {
        if (_controller == null) return;

        _physicsSO.Update();
        _effectsSO.Update();
        _audioSO.Update();
        _uiSO.Update();
        _inputSO.Update();

        EditorGUILayout.Space(10);
        _showPhysics = EditorGUILayout.Foldout(_showPhysics, "PHYSICS", true, EditorStyles.boldLabel);
        if (_showPhysics)
        {
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_maxSpeed"), 20, 190, "Max Speed");
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_maxReverseSpeed"), 10, 120, "Max Reverse Speed");
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_accelerationMultiplier"), 1, 10, "Acceleration Multiplier");
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_accelerationCurve"), new GUIContent("Acceleration Curve"));
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_maxSteeringAngle"), 10, 45, "Max Steering Angle");
            EditorGUILayout.Slider(_physicsSO.FindProperty("_steeringSpeed"), 0.1f, 1f, "Steering Speed");
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_brakeForce"), 100, 600, "Brake Force");
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_decelerationMultiplier"), 1, 10, "Deceleration Multiplier");
            EditorGUILayout.IntSlider(_physicsSO.FindProperty("_handbrakeDriftMultiplier"), 1, 10, "Drift Multiplier");
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_bodyMassCenter"), new GUIContent("Mass Center"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_pointForceLeft"), new GUIContent("Force Point Left"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_pointForceRight"), new GUIContent("Force Point Right"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_forceRotate"), new GUIContent("Rotation Force"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_frontLeftWheel"), new GUIContent("Front Left Wheel"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_frontRightWheel"), new GUIContent("Front Right Wheel"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_rearLeftWheel"), new GUIContent("Rear Left Wheel"));
            EditorGUILayout.PropertyField(_physicsSO.FindProperty("_rearRightWheel"), new GUIContent("Rear Right Wheel"));
        }

        EditorGUILayout.Space(10);
        _showEffects = EditorGUILayout.Foldout(_showEffects, "EFFECTS", true, EditorStyles.boldLabel);
        if (_showEffects)
        {
            _effectsSO.FindProperty("_useEffects").boolValue = EditorGUILayout.Toggle("Use Effects", _effectsSO.FindProperty("_useEffects").boolValue);
            if (_effectsSO.FindProperty("_useEffects").boolValue)
            {
                EditorGUILayout.PropertyField(_effectsSO.FindProperty("_rearLeftParticleSystem"), new GUIContent("Rear Left Particles"));
                EditorGUILayout.PropertyField(_effectsSO.FindProperty("_rearRightParticleSystem"), new GUIContent("Rear Right Particles"));
                EditorGUILayout.PropertyField(_effectsSO.FindProperty("_rearLeftTireSkid"), new GUIContent("Rear Left Trail"));
                EditorGUILayout.PropertyField(_effectsSO.FindProperty("_rearRightTireSkid"), new GUIContent("Rear Right Trail"));
            }
        }

        EditorGUILayout.Space(10);
        _showAudio = EditorGUILayout.Foldout(_showAudio, "AUDIO", true, EditorStyles.boldLabel);
        if (_showAudio)
        {
            _audioSO.FindProperty("_useSounds").boolValue = EditorGUILayout.Toggle("Use Sounds", _audioSO.FindProperty("_useSounds").boolValue);
            if (_audioSO.FindProperty("_useSounds").boolValue)
            {
                EditorGUILayout.PropertyField(_audioSO.FindProperty("_carEngineSound"), new GUIContent("Engine Sound"));
                EditorGUILayout.PropertyField(_audioSO.FindProperty("_tireScreechSound"), new GUIContent("Tire Screech Sound"));
            }
        }

        EditorGUILayout.Space(10);
        _showUI = EditorGUILayout.Foldout(_showUI, "UI", true, EditorStyles.boldLabel);
        if (_showUI)
        {
            _uiSO.FindProperty("_useUI").boolValue = EditorGUILayout.Toggle("Use UI", _uiSO.FindProperty("_useUI").boolValue);
            if (_uiSO.FindProperty("_useUI").boolValue)
            {
                EditorGUILayout.PropertyField(_uiSO.FindProperty("_carSpeedText"), new GUIContent("Speed Text"));
            }
        }

        EditorGUILayout.Space(10);
        _showInput = EditorGUILayout.Foldout(_showInput, "INPUT", true, EditorStyles.boldLabel);
        if (_showInput)
        {
            _inputSO.FindProperty("_useTouchControls").boolValue = EditorGUILayout.Toggle("Use Touch Controls", _inputSO.FindProperty("_useTouchControls").boolValue);
            if (_inputSO.FindProperty("_useTouchControls").boolValue)
            {
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_uIButton"), new GUIContent("UI Button"));
                /*
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_throttleButton"), new GUIContent("Throttle Button"));
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_reverseButton"), new GUIContent("Reverse Button"));
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_turnLeftButton"), new GUIContent("Turn Left Button"));
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_turnRightButton"), new GUIContent("Turn Right Button"));
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_handbrakeButton"), new GUIContent("Handbrake Button"));
                EditorGUILayout.PropertyField(_inputSO.FindProperty("_spawnButton"), new GUIContent("Spawn Button"));
                */
            }
        }

        ValidateComponents();

        _physicsSO.ApplyModifiedProperties();
        _effectsSO.ApplyModifiedProperties();
        _audioSO.ApplyModifiedProperties();
        _uiSO.ApplyModifiedProperties();
        _inputSO.ApplyModifiedProperties();
    }

    private void ValidateComponents()
    {
        if (_physics == null || _effects == null || _audio == null || _ui == null || _input == null || _teleport == null)
        {
            EditorGUILayout.HelpBox("All required components must be attached to this GameObject!", MessageType.Error);
        }
    }
}