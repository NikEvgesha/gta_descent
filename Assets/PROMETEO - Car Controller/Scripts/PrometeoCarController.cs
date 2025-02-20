/*
MESSAGE FROM CREATOR: This script was coded by Mena. You can use it in your games either these are commercial or
personal projects. You can even add or remove functions as you wish. However, you cannot sell copies of this
script by itself, since it is originally distributed as a free product.
I wish you the best for your project. Good luck!

P.S: If you need more cars, you can check my other vehicle assets on the Unity Asset Store, perhaps you could find
something useful for your game. Best regards, Mena.
*/

using System;
using UnityEngine;
using UnityEngine.UI;

public class PrometeoCarController : MonoBehaviour
{

    //CAR SETUP
    [Space(20)]
    //[Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90; //Максимальная скорость, которую может развить автомобиль в км/ч.
    [Range(10, 120)]
    public int maxReverseSpeed = 45; //Максимальная скорость, которую может развить автомобиль при движении задним ходом, в км/ч.
    [Range(1, 10)]
    public int accelerationMultiplier = 2; //Как быстро автомобиль может ускориться. 1 - это медленное ускорение, а 10 - самое быстрое.
    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27; // Максимальный угол, которого могут достичь шины при вращении рулевого колеса.
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f; // Как быстро вращается руль.
    [Space(10)]
    [Range(100, 600)]
    public int brakeForce = 350; // Сила торможения колеса.
    [Range(1, 10)]
    public int decelerationMultiplier = 2; // Как быстро автомобиль замедляется, когда пользователь не использует дроссель.
    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5; // Насколько сильно автомобиль теряет сцепление с дорогой, когда пользователь нажимает на ручной тормоз.



    [Header("Advanced Acceleration Settings")]
    public bool useExponentialAcceleration = false;
    public float accelerationCurveFactor = 2f; // Чем выше, тем более нелинейное ускорение

    private Rigidbody _rb;
    private float _currentSpeed;
    private float _inputAcceleration;

    [Space(10)]
      public Vector3 bodyMassCenter; // Это вектор, который содержит центр масс автомобиля. Я рекомендую установить это значение
                                     // в точках x = 0 и z = 0 вашего автомобиля. Вы можете выбрать нужное вам значение по оси y,
                                     // однако вы должны заметить, что чем выше это значение, тем более неустойчивым становится автомобиль.
                                     // Обычно значение y колеблется от 0 до 1,5.

    [Space(10)]
    [SerializeField] private Transform _pointForseLeft;
    [SerializeField] private Transform _pointForseRight;
    [SerializeField] private float _forseRotate = 1;

    //WHEELS

    //[Header("WHEELS")]

    /*
    Следующие переменные используются для хранения данных о колесах автомобиля. Нам нужны как игровые объекты, состоящие только из сетки, так и компоненты колес
    компоненты коллайдера колес. Компоненты коллайдера колес и 3D сетки колес не могут быть из одного и того же
    они должны быть отдельными игровыми объектами.
    */
    public GameObject frontLeftMesh;
      public WheelCollider frontLeftCollider;
      [Space(10)]
      public GameObject frontRightMesh;
      public WheelCollider frontRightCollider;
      [Space(10)]
      public GameObject rearLeftMesh;
      public WheelCollider rearLeftCollider;
      [Space(10)]
      public GameObject rearRightMesh;
      public WheelCollider rearRightCollider;

    //PARTICLE SYSTEMS

      [Space(20)]
      //[Header("EFFECTS")]
      [Space(10)]      //Следующая переменная позволяет настроить системы частиц в вашем автомобиле
    public bool useEffects = false;

    //Следующие системы частиц используются в качестве дыма из шин, когда автомобиль дрифтует.
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
      // Следующие рендеры трасс используются в качестве заносов, когда автомобиль теряет сцепление с дорогой.
      public TrailRenderer RLWTireSkid;
      public TrailRenderer RRWTireSkid;

    //SPEED TEXT (UI)

      [Space(20)]
      //[Header("UI")]
      [Space(10)]
    //Следующая переменная позволяет настроить текст пользовательского интерфейса для отображения скорости автомобиля.
    public bool useUI = false;
    public Text carSpeedText; //Используется для хранения объекта UI, который будет отображать скорость автомобиля.

    //SOUNDS

    [Space(20)]
      //[Header("Sounds")]
      [Space(10)]      //Следующая переменная позволяет настроить звуки для вашего автомобиля, например, звук двигателя или скрип шин.
    public bool useSounds = false;
    public AudioSource carEngineSound; //В этой переменной хранится звук двигателя автомобиля.
    public AudioSource tireScreechSound; // В этой переменной хранится звук визга шин (когда машина уходит в занос).
    float initialCarEngineSoundPitch; // Используется для хранения начального тона звука двигателя автомобиля.

    //CONTROLS

      [Space(20)]
      //[Header("CONTROLS")]
      [Space(10)]      
      //Следующие переменные позволяют настроить сенсорное управление для мобильных устройств.
    public bool useTouchControls = false;
      public GameObject throttleButton;
      PrometeoTouchInput throttlePTI;
      public GameObject reverseButton;
      PrometeoTouchInput reversePTI;
      public GameObject turnRightButton;
      PrometeoTouchInput turnRightPTI;
      public GameObject turnLeftButton;
      PrometeoTouchInput turnLeftPTI;
      public GameObject handbrakeButton;
      PrometeoTouchInput handbrakePTI;

    //CAR DATA

      [HideInInspector]
      public float carSpeed; // Используется для хранения скорости автомобиля.
    [HideInInspector]
    public bool isDrifting; // Используется, чтобы узнать, дрифтует автомобиль или нет.
    [HideInInspector]
    public bool isTractionLocked; // Используется, чтобы узнать, заблокировано ли сцепление автомобиля с дорогой или нет.

    //PRIVATE VARIABLES

    /*
    ВАЖНО: Следующие переменные не должны быть изменены вручную, так как их значения автоматически задаются скриптом.
    */
    Rigidbody carRigidbody; // Хранит жесткое тело автомобиля.
    float steeringAxis; // Используется, чтобы узнать, достиг ли руль максимального значения. Оно изменяется от -1 до 1.
    float throttleAxis; // Используется, чтобы узнать, достиг ли дроссель максимального значения. Оно изменяется от -1 до 1.
    float driftingAxis; 
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    bool touchControlsSetup = false;
    /*
    Следующие переменные используются для хранения информации о боковом трении колес (такие как
    extremumSlip,extremumValue, asymptoteSlip, asymptoteValue и stiffness). Мы изменим эти значения на
    чтобы автомобиль начал дрифтовать.
    */
    WheelFrictionCurve FLwheelFriction;
      float FLWextremumSlip;
      WheelFrictionCurve FRwheelFriction;
      float FRWextremumSlip;
      WheelFrictionCurve RLwheelFriction;
      float RLWextremumSlip;
      WheelFrictionCurve RRwheelFriction;
      float RRWextremumSlip;

    // Start вызывается перед обновлением первого кадра
    void Start()
    {      //В этой части мы устанавливаем значение 'carRigidbody' для жесткого тела, прикрепленного к этому
           //игровому объекту. Также мы определяем центр масс автомобиля с помощью Vector3, заданного
           //в инспекторе.
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
        _rb = carRigidbody;
        //Инициальная настройка для вычисления величины дрифта автомобиля. Эта часть может показаться немного
        //сложной, но не пугайтесь, единственное, что мы здесь делаем, это сохраняем значение по умолчанию
        //значения трения колес автомобиля по умолчанию, чтобы мы могли установить подходящее значение дрифта позже.
        FLwheelFriction = new WheelFrictionCurve ();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
      FRwheelFriction = new WheelFrictionCurve ();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
      RLwheelFriction = new WheelFrictionCurve ();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
      RRwheelFriction = new WheelFrictionCurve ();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        // Мы сохраняем начальную высоту звука автомобильного двигателя.
        if (carEngineSound != null){
          initialCarEngineSoundPitch = carEngineSound.pitch;
        }

        // Мы вызываем 2 метода внутри этого скрипта. CarSpeedUI() изменяет текст объекта UI, который хранит...
        // скорость автомобиля, а CarSounds() управляет звуками двигателя и дрифта. Оба метода вызываются
        // через 0 секунд и повторяются каждые 0,1 секунды.
        if (useUI){
          InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        }else if(!useUI){
          if(carSpeedText != null){
            carSpeedText.text = "0";
          }
        }

        if(useSounds){
          InvokeRepeating("CarSounds", 0f, 0.1f);
        }else if(!useSounds){
          if(carEngineSound != null){
            carEngineSound.Stop();
          }
          if(tireScreechSound != null){
            tireScreechSound.Stop();
          }
        }

        if(!useEffects){
          if(RLWParticleSystem != null){
            RLWParticleSystem.Stop();
          }
          if(RRWParticleSystem != null){
            RRWParticleSystem.Stop();
          }
          if(RLWTireSkid != null){
            RLWTireSkid.emitting = false;
          }
          if(RRWTireSkid != null){
            RRWTireSkid.emitting = false;
          }
        }

        if(useTouchControls){
          if(throttleButton != null && reverseButton != null &&
          turnRightButton != null && turnLeftButton != null
          && handbrakeButton != null){

            throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();
            reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();
            turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();
            turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();
            handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();
            touchControlsSetup = true;

          }else{
            String ex = "Touch controls are not completely set up. You must drag and drop your scene buttons in the" +
            " PrometeoCarController component.";
            Debug.LogWarning(ex);
          }
        }

    }
    void Update()
    {
        HandleInput();
    }
    // Обновление вызывается один раз за кадр
    void FixedUpdate()
    {

        ApplyAcceleration();
        ApplySteering();
        //CAR DATA

        // Мы определяем скорость автомобиля.
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        // Сохраните локальную скорость автомобиля по оси x. Используется, чтобы узнать, дрейфует ли автомобиль.
        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
        // Сохраните локальную скорость автомобиля по оси z. Используется для определения того, едет ли автомобиль вперед или назад.
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        //CAR PHYSICS

        /*
        Следующая часть касается контроллера автомобиля. Сначала проверяется, хочет ли пользователь использовать сенсорное управление (for
        mobile devices) или регуляторы аналогового входа (WASD + Space).

        Следующие методы вызываются каждый раз, когда нажата определенная клавиша. Например, в первом "если" мы вызываем
        метод GoForward(), если пользователь нажал клавишу W.

        В этой части кода мы указываем, что должна сделать машина, если пользователь нажмет W (throttle), S (reverse),
        A (turn left), D (turn right) or Space bar (handbrake).
        */
        if (useTouchControls && touchControlsSetup){

        if(throttlePTI.buttonPressed){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoForward();
        }
        if(reversePTI.buttonPressed){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoReverse();
        }

        if(turnLeftPTI.buttonPressed){
          TurnLeft();
        }
        if(turnRightPTI.buttonPressed){
          TurnRight();
        }
        if(handbrakePTI.buttonPressed){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          Handbrake();
        }
        if(!handbrakePTI.buttonPressed){
          RecoverTraction();
        }
        if((!throttlePTI.buttonPressed && !reversePTI.buttonPressed)){
          ThrottleOff();
        }
        if((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar){
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }
        if(!turnLeftPTI.buttonPressed && !turnRightPTI.buttonPressed && steeringAxis != 0f){
          ResetSteeringAngle();
        }

      }else{

        if(Input.GetKey(KeyCode.W)){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoForward();
        }
        if(Input.GetKey(KeyCode.S)){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoReverse();
        }

        if(Input.GetKey(KeyCode.A)){
          TurnLeft();
        }
        if(Input.GetKey(KeyCode.D)){
          TurnRight();
        }
        if(Input.GetKey(KeyCode.Space)){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          Handbrake();
        }
        if(Input.GetKeyUp(KeyCode.Space)){
          RecoverTraction();
        }
        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
          ThrottleOff();
        }
        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f){
          ResetSteeringAngle();
        }

      }


        // Мы вызываем метод AnimateWheelMeshes(), чтобы согласовать движения коллайдеров колес с 3D-сетками колес.
        AnimateWheelMeshes();

    }

    // Этот метод преобразует данные о скорости автомобиля из float в строку, а затем устанавливает текст пользовательского интерфейса carSpeedText в это значение.
    public void CarSpeedUI(){

      if(useUI){
          try{
            float absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
          }catch(Exception ex){
            Debug.LogWarning(ex);
          }
      }

    }

    // Этот метод управляет звуками автомобиля. Например, двигатель автомобиля будет звучать медленно, когда скорость автомобиля низкая, потому что
    // высота звука будет самой низкой. С другой стороны, он будет звучать быстро, когда скорость автомобиля высока, потому что
    // высота звука будет равна сумме начальной высоты звука + скорость автомобиля, деленной на 100f.
    // Кроме того, звук tireScreechSound будет воспроизводиться всякий раз, когда автомобиль начинает дрифтовать или терять сцепление с дорогой.

    public void CarSounds(){

      if(useSounds){
        try{
          if(carEngineSound != null){
            float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
            carEngineSound.pitch = engineSoundPitch;
          }
          if((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)){
            if(!tireScreechSound.isPlaying){
              tireScreechSound.Play();
            }
          }else if((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)){
            tireScreechSound.Stop();
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }
      }else if(!useSounds){
        if(carEngineSound != null && carEngineSound.isPlaying){
          carEngineSound.Stop();
        }
        if(tireScreechSound != null && tireScreechSound.isPlaying){
          tireScreechSound.Stop();
        }
      }

    }

    //
    //STEERING METHODS
    //

    //Следующий метод поворачивает передние колеса автомобиля влево. Скорость этого движения будет зависеть от переменной steeringSpeed.
    public void TurnLeft(){
        carRigidbody.AddForceAtPosition(carRigidbody.transform.position - _pointForseLeft.localPosition, new Vector3( 0f, -_forseRotate, 0f));
        carRigidbody.AddForceAtPosition(carRigidbody.transform.position - _pointForseRight.localPosition, new Vector3(0f, _forseRotate, 0f));

        //this.GetComponent<Rigidbody>().AddForce(0f, 5000f, 0f);
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis < -1f){
        steeringAxis = -1f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //Следующий метод поворачивает передние колеса автомобиля вправо. Скорость этого движения будет зависеть от переменной steeringSpeed.
    public void TurnRight()
    {
        carRigidbody.AddForceAtPosition(carRigidbody.transform.position - _pointForseRight.localPosition, new Vector3(0f, -_forseRotate, 0f));
        carRigidbody.AddForceAtPosition(carRigidbody.transform.position - _pointForseLeft.localPosition, new Vector3(0f, _forseRotate, 0f));

        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      if(steeringAxis > 1f){
        steeringAxis = 1f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //Следующий метод переводит передние колеса автомобиля в положение по умолчанию(rotation = 0). Скорость этого движения будет зависеть от
    // от переменной steeringSpeed.
    public void ResetSteeringAngle(){
      if(steeringAxis < 0f){
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
      }else if(steeringAxis > 0f){
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
      }
      if(Mathf.Abs(frontLeftCollider.steerAngle) < 1f){
        steeringAxis = 0f;
      }
      var steeringAngle = steeringAxis * maxSteeringAngle;
      frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
      frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // Этот метод согласует положение и вращение WheelColliders с WheelMeshes.
    void AnimateWheelMeshes(){
      try{
        Quaternion FLWRotation;
        Vector3 FLWPosition;
        frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
        frontLeftMesh.transform.position = FLWPosition;
        frontLeftMesh.transform.rotation = FLWRotation;

        Quaternion FRWRotation;
        Vector3 FRWPosition;
        frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
        frontRightMesh.transform.position = FRWPosition;
        frontRightMesh.transform.rotation = FRWRotation;

        Quaternion RLWRotation;
        Vector3 RLWPosition;
        rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
        rearLeftMesh.transform.position = RLWPosition;
        rearLeftMesh.transform.rotation = RLWRotation;

        Quaternion RRWRotation;
        Vector3 RRWPosition;
        rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
        rearRightMesh.transform.position = RRWPosition;
        rearRightMesh.transform.rotation = RRWRotation;
      }catch(Exception ex){
        Debug.LogWarning(ex);
      }
    }

    //
    //ENGINE AND BRAKING METHODS
    //

    // This method apply positive torque to the wheels in order to go forward.
    public void GoForward(){
      //If the forces aplied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car is losing traction, then the car will start emitting particle systems.
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      // The following part sets the throttle power to 1 smoothly.
      throttleAxis = throttleAxis + (Time.deltaTime * 3f);
      if(throttleAxis > 1f){
        throttleAxis = 1f;
      }
      //If the car is going backwards, then apply brakes in order to avoid strange
      //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
      //is safe to apply positive torque to go forward.
      if(localVelocityZ < -1f){
        Brakes();
      }else{
        if(Mathf.RoundToInt(carSpeed) < maxSpeed){
          //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          // If the maxSpeed has been reached, then stop applying torque to the wheels.
          // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
          // could be a bit higher than expected.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    // This method apply negative torque to the wheels in order to go backwards.
    public void GoReverse(){
      //If the forces aplied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car is losing traction, then the car will start emitting particle systems.
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      // The following part sets the throttle power to -1 smoothly.
      throttleAxis = throttleAxis - (Time.deltaTime * 3f);
      if(throttleAxis < -1f){
        throttleAxis = -1f;
      }
      //If the car is still going forward, then apply brakes in order to avoid strange
      //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
      //is safe to apply negative torque to go reverse.
      if(localVelocityZ > 1f){
        Brakes();
      }else{
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
          //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
          // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
          // could be a bit higher than expected.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
    public void ThrottleOff(){
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
    }

    // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
    // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
    // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
    public void DecelerateCar(){
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      // The following part resets the throttle power to 0 smoothly.
      if(throttleAxis != 0f){
        if(throttleAxis > 0f){
          throttleAxis = throttleAxis - (Time.deltaTime * 10f);
        }else if(throttleAxis < 0f){
            throttleAxis = throttleAxis + (Time.deltaTime * 10f);
        }
        if(Mathf.Abs(throttleAxis) < 0.15f){
          throttleAxis = 0f;
        }
      }
      carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
      // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
      // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
      // also cancel the invoke of this method.
      if(carRigidbody.velocity.magnitude < 0.25f){
        carRigidbody.velocity = Vector3.zero;
        CancelInvoke("DecelerateCar");
      }
    }

    // This function applies brake torque to the wheels according to the brake force given by the user.
    public void Brakes(){
      frontLeftCollider.brakeTorque = brakeForce;
      frontRightCollider.brakeTorque = brakeForce;
      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;
    }

    // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
    // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
    // it is high, then you could make the car to feel like going on ice.
    public void Handbrake(){
      CancelInvoke("RecoverTraction");
      // We are going to start losing traction smoothly, there is were our 'driftingAxis' variable takes
      // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
      // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
      driftingAxis = driftingAxis + (Time.deltaTime);
      float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

      if(secureStartingPoint < FLWextremumSlip){
        driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
      }
      if(driftingAxis > 1f){
        driftingAxis = 1f;
      }
      //If the forces aplied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car lost its traction, then the car will start emitting particle systems.
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
      }else{
        isDrifting = false;
      }
      //If the 'driftingAxis' value is not 1f, it means that the wheels have not reach their maximum drifting
      //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
      // = 1f.
      if(driftingAxis < 1f){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;
      }

      // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
      // and, as a consequense, the car starts to emit trails to simulate the wheel skids.
      isTractionLocked = true;
      DriftCarPS();

    }

    // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
    // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
    public void DriftCarPS(){

      if(useEffects){
        try{
          if(isDrifting){
            RLWParticleSystem.Play();
            RRWParticleSystem.Play();
          }else if(!isDrifting){
            RLWParticleSystem.Stop();
            RRWParticleSystem.Stop();
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }

        try{
          if((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f){
            RLWTireSkid.emitting = true;
            RRWTireSkid.emitting = true;
          }else {
            RLWTireSkid.emitting = false;
            RRWTireSkid.emitting = false;
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }
      }else if(!useEffects){
        if(RLWParticleSystem != null){
          RLWParticleSystem.Stop();
        }
        if(RRWParticleSystem != null){
          RRWParticleSystem.Stop();
        }
        if(RLWTireSkid != null){
          RLWTireSkid.emitting = false;
        }
        if(RRWTireSkid != null){
          RRWTireSkid.emitting = false;
        }
      }

    }

    // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
    public void RecoverTraction(){
      isTractionLocked = false;
      driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
      if(driftingAxis < 0f){
        driftingAxis = 0f;
      }

      //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
      //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
      // car's grip.
      if(FLwheelFriction.extremumSlip > FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        Invoke("RecoverTraction", Time.deltaTime);

      }else if (FLwheelFriction.extremumSlip < FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        driftingAxis = 0f;
      }
    }

    public Transform SpawnPoint; // Начальная точка телепорта
    public Action UseTeleport;
    public void TeleportCar(Transform spawn,bool onInertion = false)
    {
        SpawnPoint = spawn;

        Vector3 velocity = onInertion ? carRigidbody.velocity:Vector3.zero ; // Сохраняем скорость
        Vector3 angularVelocity = onInertion ? carRigidbody.angularVelocity : Vector3.zero; // Сохраняем вращение

        carRigidbody.position = SpawnPoint.position; // Телепортируем машину
        carRigidbody.rotation = SpawnPoint.rotation; // Выставляем правильное направление

        carRigidbody.velocity = SpawnPoint.forward * velocity.magnitude; // Применяем скорость в новом направлении
        carRigidbody.angularVelocity = angularVelocity; // Сохраняем вращение
        UseTeleport?.Invoke();
    }

    void HandleInput()
    {
        _inputAcceleration = Input.GetAxis("Vertical");
    }

    void ApplyAcceleration()
    {
        if (_inputAcceleration > 0)
        {
            float speedFactor = Mathf.Clamp01(_rb.velocity.magnitude / maxSpeed);
            float acceleration = useExponentialAcceleration
                ? accelerationMultiplier * Mathf.Pow(1 - speedFactor, accelerationCurveFactor)
                : accelerationMultiplier * (1 - speedFactor);

            _rb.AddForce(transform.forward * acceleration * _inputAcceleration, ForceMode.Acceleration);
        }
        else if (_inputAcceleration < 0)
        {
            if (_rb.velocity.magnitude < maxReverseSpeed)
            {
                _rb.AddForce(transform.forward * accelerationMultiplier * _inputAcceleration, ForceMode.Acceleration);
            }
        }
        else
        {
            _rb.velocity *= 1 - (Time.fixedDeltaTime * decelerationMultiplier);
        }
    }

    void ApplySteering()
    {
        float steeringInput = Input.GetAxis("Horizontal");
        float steeringAngle = steeringInput * maxSteeringAngle;
        transform.Rotate(Vector3.up, steeringAngle * steeringSpeed * Time.fixedDeltaTime);
    }
}
