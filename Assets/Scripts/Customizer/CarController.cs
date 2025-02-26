using UnityEngine;
using UnityEngine.EventSystems;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;

    // Настройки физики
    [SerializeField] private float motorForce = 2500f;
    [SerializeField] private float brakeForce = 5000f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float airControlForce = 1f; // Уменьшено до 1

    // Аудио
    public AudioSource engineSound;
    private float initialPitch = 1f;
    [SerializeField] private float pitchMultiplier = 3f;

    // Эффекты
    public ParticleSystem smokeParticlesLeft, smokeParticlesRight;
    public TrailRenderer trailLeft, trailRight;

    // UI для мобильных устройств
    public GameObject forwardButton, reverseButton, leftButton, rightButton, brakeButton;

    // Состояние
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
        initialPitch = engineSound.pitch;
        SetupMobileControls();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        GetInput();
        HandleMotor();
        HandleSteering();
        HandleAirControl();
        UpdateWheels();
        UpdateAudio();
        UpdateEffects();
    }

    void CheckGrounded()
    {
        isGrounded = frontLeftWheel.isGrounded || frontRightWheel.isGrounded ||
                     rearLeftWheel.isGrounded || rearRightWheel.isGrounded;
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);
    }

    void HandleMotor()
    {
        if (isGrounded)
        {
            frontLeftWheel.motorTorque = verticalInput * motorForce;
            frontRightWheel.motorTorque = verticalInput * motorForce;
            rearLeftWheel.motorTorque = verticalInput * motorForce;
            rearRightWheel.motorTorque = verticalInput * motorForce;

            float currentBrakeForce = isBraking ? brakeForce : 0f;
            ApplyBraking(currentBrakeForce);
        }
    }

    void HandleSteering()
    {
        float steer = maxSteerAngle * horizontalInput;
        frontLeftWheel.steerAngle = steer;
        frontRightWheel.steerAngle = steer;
    }

    void HandleAirControl()
    {
        if (!isGrounded && (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f))
        {
            // Инвертированное управление вперед/назад в полете
            Vector3 airTorque = new Vector3(-verticalInput * airControlForce, horizontalInput * airControlForce, 0f);
            rb.AddTorque(airTorque * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    void ApplyBraking(float brakeForce)
    {
        frontLeftWheel.brakeTorque = brakeForce;
        frontRightWheel.brakeTorque = brakeForce;
        rearLeftWheel.brakeTorque = brakeForce;
        rearRightWheel.brakeTorque = brakeForce;
    }

    void UpdateWheels()
    {
        UpdateWheelPos(frontLeftWheel);
        UpdateWheelPos(frontRightWheel);
        UpdateWheelPos(rearLeftWheel);
        UpdateWheelPos(rearRightWheel);
    }

    void UpdateWheelPos(WheelCollider wheel)
    {
        if (wheel.transform.childCount == 0) return;

        Transform visualWheel = wheel.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        wheel.GetWorldPose(out position, out rotation);
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    void UpdateAudio()
    {
        float speed = rb.velocity.magnitude;
        engineSound.pitch = initialPitch + (speed * pitchMultiplier / 50f);
        engineSound.volume = Mathf.Clamp01(0.5f + speed / 15f);
    }

    void UpdateEffects()
    {
        bool isDrifting = Mathf.Abs(horizontalInput) > 0.7f && rb.velocity.magnitude > 7f;
        bool isJumping = !isGrounded;

        if (isBraking || isDrifting || isJumping)
        {
            if (!smokeParticlesLeft.isPlaying) smokeParticlesLeft.Play();
            if (!smokeParticlesRight.isPlaying) smokeParticlesRight.Play();
            trailLeft.emitting = true;
            trailRight.emitting = true;
        }
        else
        {
            smokeParticlesLeft.Stop();
            smokeParticlesRight.Stop();
            trailLeft.emitting = false;
            trailRight.emitting = false;
        }
    }

    void SetupMobileControls()
    {
        SetupButton(forwardButton, () => verticalInput = 1f, () => verticalInput = 0f);
        SetupButton(reverseButton, () => verticalInput = -1f, () => verticalInput = 0f);
        SetupButton(leftButton, () => horizontalInput = -1f, () => horizontalInput = 0f);
        SetupButton(rightButton, () => horizontalInput = 1f, () => horizontalInput = 0f);
        SetupButton(brakeButton, () => isBraking = true, () => isBraking = false);
    }

    void SetupButton(GameObject button, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => onDown());
        trigger.triggers.Add(pointerDown);

        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => onUp());
        trigger.triggers.Add(pointerUp);
    }
}