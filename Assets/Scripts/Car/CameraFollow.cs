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
    private bool isRotating = true;
    private float timeSinceLastInput = 0f;
    private bool forceInstantUpdate = false; 
    private float forceUpdateTime = 0f; // Таймер на несколько кадров
    private float forceUpdateDuration = 0.1f; // Время в секундах для фиксации

    private bool isDragging = false;
    private Vector2 lastTouchPosition; 
    [Header("Collision Settings")]
    public float collisionOffset = 0.2f;  // Отступ, чтобы камера не "прилипала" к стене
    public LayerMask collisionLayers;     // Слои для проверки столкновений

    [Header("Back Camera Settings")]
    public Transform backCameraPoint;     // Точка, куда камера должна переходить при минимальном движении
    public float minimalMovementThreshold = 0.1f; // Порог скорости, ниже которого считается, что движение минимально

    // Дополнительный множитель для плавного перехода (при желании можно настроить)
    public float transitionMultiplier = 1f; 
    [Header("Rotation Smoothing")]
    public float rotationSmoothTime = 0.2f;  // Время сглаживания поворота камеры
    private Vector2 currentRotation;         // Текущий сглаженный угол камеры (x = yaw, y = pitch)
    private Vector2 rotationVelocity;        // Вспомогательные переменные для SmoothDampAngle
    public float sphereCastRadius = 0.2f; // Радиус проверки столкновений

    [Header("Camera Angle Limits")]
    public float minCameraPitch = -10f; // Минимальный угол поворота камеры (например, -10°)
    public float maxCameraPitch = 30f;  // Максимальный угол поворота камеры (например, 30°)

    // Функция для нормализации угла (приводит значение к диапазону от -180 до 180)

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
        float inputX = 0f;
        float inputY = 0f;

        if (YG2.envir.isDesktop)
        {
            // Управление для ПК через CarInput
            float mouseX = carInput.MouseX * mouseRotationSpeed;
            float mouseY = carInput.MouseY * mouseRotationSpeed;
            if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
            {
                inputX = mouseX;
                inputY = mouseY;
                userInput = true;
            }
        }

        // Управление для мобильных устройств
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
                    inputX = delta.x * touchRotationSpeed;
                    inputY = delta.y * touchRotationSpeed;
                    lastTouchPosition = touch.position;
                    userInput = true;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
        }

        if (userInput)
        {
            // Обновляем желаемые углы на основе ввода пользователя
            rotation.x += inputX;
            rotation.y -= inputY;
            rotation.y = Mathf.Clamp(rotation.y, 5f, 60f);

            timeSinceLastInput = 0f;
            isRotating = true;
        }
        else
        {
            timeSinceLastInput += Time.fixedDeltaTime;
            if (timeSinceLastInput >= returnDelay)
            {
                isRotating = false;
            }
        }

        // Если ввода нет, плавно возвращаем камеру к направлению движения
        if (!isRotating && carTransform)
        {
            Rigidbody rb = carTransform.GetComponent<Rigidbody>();
            float speed = rb != null ? rb.velocity.magnitude : 0f;
            float targetRotationX;

            if (speed < minimalMovementThreshold)
            {
                // При минимальном движении камера возвращается к направлению машины
                targetRotationX = carTransform.eulerAngles.y;
            }
            else
            {
                targetRotationX = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
            }

            // Интерполируем желаемый угол камеры по горизонтали
            rotation.x = Mathf.LerpAngle(rotation.x, targetRotationX, Time.fixedDeltaTime * rotationReturnSpeed);
        }

        // Плавно обновляем текущий сглаженный угол камеры (currentRotation)
        currentRotation.x = Mathf.SmoothDampAngle(currentRotation.x, rotation.x, ref rotationVelocity.x, rotationSmoothTime);
        currentRotation.y = Mathf.SmoothDampAngle(currentRotation.y, rotation.y, ref rotationVelocity.y, rotationSmoothTime);
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
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    void UpdateCameraPosition(bool instant = false)
    {
        if (!carTransform) return;

        // Получаем нормализованный угол наклона машины по оси X (pitch)
        float carPitch = NormalizeAngle(carTransform.eulerAngles.x);

        // Пользовательский ввод:
        // rotation.y отвечает за вертикальное смещение (дополнительный pitch),
        // rotation.x — за горизонтальный поворот (yaw).
        float finalPitch = carPitch + rotation.y;
        // Ограничиваем итоговый угол камеры по вертикали
        finalPitch = Mathf.Clamp(finalPitch, minCameraPitch, maxCameraPitch);
        float finalYaw = rotation.x; // используем только пользовательский yaw

        // Итоговая ориентация камеры: учитываем только pitch машины (с ограничением), остальные оси берём из ввода
        Quaternion desiredCameraRotation = Quaternion.Euler(finalPitch, finalYaw, 0);

        // Вычисляем позицию камеры: позиция задаётся на основе итоговой ориентации,
        // расстояния от машины и фиксированной высоты (сохраняя world-up для высоты)
        Vector3 desiredPosition = carTransform.position - (desiredCameraRotation * Vector3.forward * distance)
                                  + (desiredCameraRotation * Vector3.up * height);

        // Проверка столкновений между машиной и желаемой позицией камеры
        RaycastHit hit;
        Vector3 targetPosition = desiredPosition;
        if (Physics.Linecast(carTransform.position, desiredPosition, out hit, collisionLayers))
        {
            targetPosition = hit.point + hit.normal * collisionOffset;
        }

        if (instant || !Application.isPlaying)
        {
            transform.position = targetPosition;
            velocity = Vector3.zero;
            transform.rotation = desiredCameraRotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredCameraRotation, Time.deltaTime / rotationSmoothTime);
        }
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