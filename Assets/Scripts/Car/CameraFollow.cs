using UnityEngine;
using YG;

[ExecuteAlways]
public class CameraFollow : MonoBehaviour
{
    public Transform carTransform; // Машина
    public float mouseRotationSpeed = 3f; // Чувствительность поворота для мыши
    public float touchRotationSpeed = 0.1f; // Чувствительность поворота для тач-управления
    public float distance = 5f; // Расстояние до машины
    public float height = 2f; // Высота камеры над машиной
    public float smoothTime = 0.1f; // Сглаживание позиции
    public float rotationReturnSpeed = 2f; // Скорость возврата камеры
    public float returnDelay = 2f; // Задержка перед возвратом камеры (в секундах)

    [Header("Camera Offset")]
    [Tooltip("Смещение точки, на которую смотрит камера (в локальных координатах машины)")]
    public Vector3 lookAtOffset = new Vector3(0f, 0f, 0f); // Смещение центра взгляда

    [Header("Mobile Controls")]
    public RectTransform touchArea; // Прозрачный спрайт для управления камерой на мобильных устройствах

    [Header("Input Reference")]
    public CarInput carInput; // Ссылка на скрипт CarInput

    private Vector2 rotation = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private bool isRotating = false;
    private float timeSinceLastInput = 0f;
    private bool forceInstantUpdate = false; 
    private float forceUpdateTime = 0f; // Таймер на несколько кадров
    private float forceUpdateDuration = 0.1f; // Время в секундах для фиксации

    private bool isDragging = false;
    private Vector2 lastTouchPosition;
    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            Settings.instance.ChangeMouseSensitivity += ChangeMouseSensitivity;
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            if (carTransform != null)
                carTransform.GetComponent<CarTeleport>().TeleportStart -= TeleportCamera;
            Settings.instance.ChangeMouseSensitivity -= ChangeMouseSensitivity;
        }
    }

    void Start()
    {
        if (!carInput)
        {
            carInput = FindObjectOfType<CarInput>();
            carTransform = carInput.transform;
            if (!carInput) Debug.LogError("CarInput не найден на сцене!");
        }
        if (Application.isPlaying)
        {
            touchArea = carInput.GetCameraArea();
            carTransform.GetComponent<CarTeleport>().TeleportStart += TeleportCamera;
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateCameraPosition();
        }
    }

    void FixedUpdate()
    {
        if (Application.isPlaying)
        {
            HandleCameraRotation();
            UpdateCameraPosition();
        }
        /*else if (forceInstantUpdate)
        {
            InstantUpdateCameraPosition();
        }*/
    }
    void LateUpdate()
    {
        if (forceInstantUpdate)
        {
            InstantUpdateCameraPosition();

            // Поддерживаем мгновенное обновление на протяжении нескольких кадров
            forceUpdateTime -= Time.deltaTime;
            if (forceUpdateTime <= 0)
            {
                forceInstantUpdate = false;
            }
        }
    }


    void HandleCameraRotation()
    {
        if (!carInput) return;

        bool userInput = false;
        if (YG2.envir.isDesktop)
        {
            
            // Управление для ПК через CarInput
            float mouseX = carInput.MouseX * mouseRotationSpeed;
            float mouseY = carInput.MouseY * mouseRotationSpeed;
            if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
            {
                rotation.x += mouseX;
                rotation.y -= mouseY;
                rotation.y = Mathf.Clamp(rotation.y, 5f, 60f);
                userInput = true;
            }

        }

        // Управление для мобильных устройств (через прозрачный спрайт)
        if (Input.touchCount > 0 && touchArea != null)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touchPos))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    lastTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector2 delta = touch.position - lastTouchPosition;
                    rotation.x += delta.x * touchRotationSpeed;
                    rotation.y -= delta.y * touchRotationSpeed;
                    rotation.y = Mathf.Clamp(rotation.y, 5f, 60f);

                    lastTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }

                userInput = true;
            }
        }


        // Логика задержки перед возвратом
        if (userInput)
        {
            isRotating = true;
            timeSinceLastInput = 0f;
        }
        else
        {
            timeSinceLastInput += Time.fixedDeltaTime;
            if (timeSinceLastInput >= returnDelay)
            {
                isRotating = false;
            }
        }

        // Если не вращаем и прошла задержка, подстраиваем камеру под движение машины
        if (!isRotating && carTransform)
        {
            Vector3 carVelocity = carTransform.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero;
            float speed = carVelocity.magnitude;

            float targetRotationX;
            if (speed < 0.1f) // Если машина почти стоит
            {
                targetRotationX = carTransform.eulerAngles.y; // Камера за машиной
            }
            else // Если машина движется
            {
                targetRotationX = Mathf.Atan2(carVelocity.x, carVelocity.z) * Mathf.Rad2Deg;
            }

            rotation.x = Mathf.LerpAngle(rotation.x, targetRotationX, Time.fixedDeltaTime * rotationReturnSpeed);
        }
    }
    public void TeleportCamera()
    {
        forceInstantUpdate = true;
        forceUpdateTime = forceUpdateDuration; // Активируем принудительное обновление
        rotation.x = carTransform.eulerAngles.y;
        InstantUpdateCameraPosition();
    }


    void InstantUpdateCameraPosition()
    {
        if (!carTransform) return;

        Quaternion rotationQuat = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 targetPosition = carTransform.position - (rotationQuat * Vector3.forward * distance) + (Vector3.up * height);

        // Принудительно ставим камеру на место
        transform.position = targetPosition;

        // Сразу смотрим на нужную точку
        Vector3 lookAtPoint = carTransform.position + carTransform.TransformDirection(lookAtOffset);
        transform.LookAt(lookAtPoint);
    }

    void UpdateCameraPosition(bool instant = false)
    {
        if (!carTransform) return;

        Quaternion rotationQuat = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 targetPosition = carTransform.position - (rotationQuat * Vector3.forward * distance) + (Vector3.up * height);

        if (instant || !Application.isPlaying)
        {
            transform.position = targetPosition;
            velocity = Vector3.zero; // Останавливаем плавное движение
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        Vector3 lookAtPoint = carTransform.position + carTransform.TransformDirection(lookAtOffset);
        transform.LookAt(lookAtPoint);
    }

    void OnValidate()
    {
        UpdateCameraPosition();
    }

    private void ChangeMouseSensitivity(float sens)
    {
        mouseRotationSpeed = Mathf.Lerp(1f, 10f, sens);
        touchRotationSpeed = Mathf.Lerp(0.1f, 0.5f, sens);
    }
}