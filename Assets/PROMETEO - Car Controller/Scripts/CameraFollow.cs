using UnityEngine;

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

    private void OnEnable()
    {
        if (Application.isPlaying)
            Settings.instance.ChangeMouseSensitivity += ChangeMouseSensitivity;
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            Settings.instance.ChangeMouseSensitivity -= ChangeMouseSensitivity;
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
    }

    void HandleCameraRotation()
    {
        if (!carInput) return;

        bool userInput = false;

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

        // Управление для мобильных устройств (через прозрачный спрайт)
        if (Input.touchCount > 0 && touchArea != null)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touchPos))
            {
                rotation.x += touch.deltaPosition.x * touchRotationSpeed;
                rotation.y -= touch.deltaPosition.y * touchRotationSpeed;
                rotation.y = Mathf.Clamp(rotation.y, 5f, 60f);
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

    void UpdateCameraPosition()
    {
        if (!carTransform) return;

        // Вычисляем позицию камеры
        Quaternion rotationQuat = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 targetPosition = carTransform.position - (rotationQuat * Vector3.forward * distance) + (Vector3.up * height);
        transform.position = Application.isPlaying ? Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime) : targetPosition;

        // Вычисляем точку, на которую смотрит камера, с учётом смещения
        Vector3 lookAtPoint = carTransform.position + carTransform.TransformDirection(lookAtOffset);
        transform.LookAt(lookAtPoint);
    }

    public void TeleportCamera()
    {
        rotation.x = carTransform.eulerAngles.y;
        transform.position = carTransform.position - (Quaternion.Euler(rotation.y, rotation.x, 0) * Vector3.forward * distance) + (Vector3.up * height);
        transform.LookAt(carTransform.position + carTransform.TransformDirection(lookAtOffset));
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