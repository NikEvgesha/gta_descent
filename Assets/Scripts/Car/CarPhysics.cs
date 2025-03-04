using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class CarPhysics : MonoBehaviour
{
    #region Constants
    private const float DRIFT_VELOCITY_THRESHOLD = 2.5f;
    private const float MIN_SPEED_THRESHOLD = 0.25f;
    #endregion

    #region Serialized Fields
    [SerializeField, Range(20, 190)] private int _maxSpeed = 90;
    [SerializeField, Range(10, 120)] private int _maxReverseSpeed = 45;
    [SerializeField, Range(1, 100)] private float _accelerationMultiplier = 10f;
    [SerializeField] private AnimationCurve _accelerationCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField, Range(10, 45)] private int _maxSteeringAngle = 27;
    [SerializeField, Range(0.1f, 1f)] private float _steeringSpeed = 0.5f;
    //[SerializeField, Range(100, 600)] private int _brakeForce = 350;
    [SerializeField, Range(1, 10)] private int _decelerationMultiplier = 2;
    [SerializeField, Range(1, 10)] private int _handbrakeDriftMultiplier = 5;
    [SerializeField] private Vector3 _bodyMassCenter = Vector3.zero;
    [SerializeField] private Transform _pointForceLeft;
    [SerializeField] private Transform _pointForceRight;
    [SerializeField] private float _forceRotate = 1f;

    [SerializeField] private Wheel _frontLeftWheel;
    [SerializeField] private Wheel _frontRightWheel;
    [SerializeField] private Wheel _rearLeftWheel;
    [SerializeField] private Wheel _rearRightWheel;
    #endregion

    #region Public Properties
    public float CarSpeed { get; private set; }
    public float LocalVelocityX { get; private set; }
    public bool IsDrifting { get; private set; }
    public bool IsTractionLocked { get; private set; }
    public bool IsOnGround { get; private set; }
    #endregion

    #region Private Fields
    private Rigidbody _rb;
    private List<Wheel> _wheels;
    private float _steeringAxis;
    private float _driftingAxis;
    private bool _deceleratingCar;
    private WheelFrictionCurve[] _wheelFrictions;
    private float[] _defaultExtremumSlips;
    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _bodyMassCenter;

        _wheels = new List<Wheel> { _frontLeftWheel, _frontRightWheel, _rearLeftWheel, _rearRightWheel };
        _wheelFrictions = new WheelFrictionCurve[4];
        _defaultExtremumSlips = new float[4];
        for (int i = 0; i < _wheels.Count; i++)
        {
            if (_wheels[i] != null && _wheels[i].Collider != null)
            {
                _wheelFrictions[i] = _wheels[i].Collider.sidewaysFriction;
                _defaultExtremumSlips[i] = _wheelFrictions[i].extremumSlip;
                WheelFrictionCurve friction = _wheels[i].Collider.sidewaysFriction;

                if (i < 2) // Передние колеса
                {
                    friction.extremumSlip *= 0.8f; // Улучшаем сцепление передних колес
                    friction.asymptoteSlip *= 0.8f;
                }
                else // Задние колеса
                {
                    friction.extremumSlip *= 1.2f; // Позволяем задним колесам больше скользить
                    friction.asymptoteSlip *= 1.2f;
                }

                _wheels[i].Collider.sidewaysFriction = friction;
            }
            else
            {
                Debug.LogWarning($"Wheel {i} is not properly set up!");
            }
        }

    }

    private void FixedUpdate()
    {
        CheckGround();
        UpdatePhysics();
        AnimateWheels();
        if (Mathf.Abs(LocalVelocityX) > 0.5f) // Если занос больше 0.5 м/с
        {
            Vector3 counterForce = -transform.right * (LocalVelocityX * 2f);
            _rb.AddForce(counterForce, ForceMode.Acceleration);
        }
    }

    public void ApplyAcceleration(bool throttle, bool reverse)
    {
        if (!IsOnGround)
        {
            //Debug.Log("Car is not on ground, cannot accelerate.");
            return;
        }

        float input = throttle ? 1f : reverse ? -1f : 0f;
        //Debug.Log($"Acceleration input: {input}");

        if (input == 0)
        {
            if (!_deceleratingCar) Decelerate();
            return;
        }

        float speedFactor = _rb.velocity.magnitude / (input > 0 ? _maxSpeed : _maxReverseSpeed);
        float acceleration = _accelerationMultiplier * _accelerationCurve.Evaluate(speedFactor);
        Vector3 force = transform.forward * acceleration * input;
        //Debug.Log($"Applying force: {force}");

        _rb.AddForce(force, ForceMode.Acceleration);

        Quaternion targetTilt = Quaternion.Euler(input * 5f, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetTilt, Time.deltaTime * 2f);

        UpdateDriftState();
    }

    public void ApplySteering(bool turnLeft, bool turnRight)
    {
        float direction = turnLeft ? -1f : turnRight ? 1f : 0f;
        _steeringAxis = Mathf.MoveTowards(_steeringAxis, direction, Time.deltaTime * 10f * _steeringSpeed);
        float speedFactor = Mathf.Clamp01(_rb.velocity.magnitude / _maxSpeed);
        float dynamicSteeringAngle = Mathf.Lerp(_maxSteeringAngle, _maxSteeringAngle * 0.5f, speedFactor);
        float steeringAngle = _steeringAxis * dynamicSteeringAngle;
        float gripBoost = 1f - Mathf.Abs(_steeringAxis) * 2f; // Чем сильнее поворот, тем выше сцепление

        WheelFrictionCurve frontLeftFriction = _frontLeftWheel.Collider.sidewaysFriction;
        WheelFrictionCurve frontRightFriction = _frontRightWheel.Collider.sidewaysFriction;

        frontLeftFriction.extremumSlip *= gripBoost;
        frontRightFriction.extremumSlip *= gripBoost;

        _frontLeftWheel.Collider.sidewaysFriction = frontLeftFriction;
        _frontRightWheel.Collider.sidewaysFriction = frontRightFriction;

        _rb.AddForce(transform.right * _steeringAxis * _rb.velocity.magnitude * 0.05f, ForceMode.Acceleration);
        if (_frontLeftWheel != null && _frontLeftWheel.Collider != null)
            _frontLeftWheel.Collider.steerAngle = Mathf.Lerp(_frontLeftWheel.Collider.steerAngle, steeringAngle, _steeringSpeed);
        if (_frontRightWheel != null && _frontRightWheel.Collider != null)
            _frontRightWheel.Collider.steerAngle = Mathf.Lerp(_frontRightWheel.Collider.steerAngle, steeringAngle, _steeringSpeed);

        if (!IsOnGround)
        {
            _rb.AddTorque(Vector3.up * direction * _forceRotate * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    public void ApplyHandbrake(bool handbrake)
    {
        if (handbrake)
        {
            _driftingAxis = Mathf.MoveTowards(_driftingAxis, 1f, Time.deltaTime);
            IsTractionLocked = true;
            for (int i = 0; i < _wheels.Count; i++)
            {
                if (_wheels[i] != null && _wheels[i].Collider != null)
                {
                    _wheelFrictions[i].extremumSlip = _defaultExtremumSlips[i] * _handbrakeDriftMultiplier * _driftingAxis;
                    _wheels[i].Collider.sidewaysFriction = _wheelFrictions[i];
                }
            }
        }
        else
        {
            _driftingAxis = Mathf.MoveTowards(_driftingAxis, 0f, Time.deltaTime / 1.5f);
            IsTractionLocked = false;
            for (int i = 0; i < _wheels.Count; i++)
            {
                if (_wheels[i] != null && _wheels[i].Collider != null)
                {
                    _wheelFrictions[i].extremumSlip = Mathf.Lerp(_defaultExtremumSlips[i], _defaultExtremumSlips[i] * _handbrakeDriftMultiplier, _driftingAxis);
                    _wheels[i].Collider.sidewaysFriction = _wheelFrictions[i];
                }
            }
        }
        UpdateDriftState();
    }

    private void CheckGround()
    {
        IsOnGround = false;
        foreach (var wheel in _wheels)
        {
            if (wheel != null && wheel.Collider != null && wheel.Collider.isGrounded)
            {
                //Debug.Log($"{wheel.Collider.gameObject.name} is grounded.");
                IsOnGround = true;
                break;
            }
            else if (wheel != null && wheel.Collider != null)
            {
                //Debug.Log($"{wheel.Collider.gameObject.name} is NOT grounded.");
            }
        }
        //Debug.Log($"IsOnGround: {IsOnGround}");
    }

    private void UpdatePhysics()
    {
        if (_frontLeftWheel != null && _frontLeftWheel.Collider != null)
            CarSpeed = (2 * Mathf.PI * _frontLeftWheel.Collider.radius * _frontLeftWheel.Collider.rpm * 60) / 1000;
        LocalVelocityX = transform.InverseTransformDirection(_rb.velocity).x;
    }

    private void Decelerate()
    {
        _deceleratingCar = true;
        _rb.velocity *= 1f / (1f + (0.025f * _decelerationMultiplier));
        foreach (var wheel in _wheels)
        {
            if (wheel != null && wheel.Collider != null) wheel.Collider.motorTorque = 0f;
        }

        if (_rb.velocity.magnitude < MIN_SPEED_THRESHOLD)
        {
            _rb.velocity = Vector3.zero;
            _deceleratingCar = false;
        }
        UpdateDriftState();
    }

    private void UpdateDriftState()
    {
        IsDrifting = Mathf.Abs(LocalVelocityX) > DRIFT_VELOCITY_THRESHOLD;
    }

    private void AnimateWheels()
    {
        foreach (var wheel in _wheels)
        {
            if (wheel != null && wheel.Collider != null && wheel.Mesh != null)
            {
                wheel.Collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
                wheel.Mesh.transform.SetPositionAndRotation(pos, rot);
            }
        }
    }

    [System.Serializable]
    public class Wheel
    {
        [SerializeField] private GameObject _mesh;
        [SerializeField] private WheelCollider _collider;

        public GameObject Mesh => _mesh;
        public WheelCollider Collider => _collider;
    }
}