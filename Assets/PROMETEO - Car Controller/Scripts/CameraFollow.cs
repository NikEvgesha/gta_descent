using UnityEngine;

[ExecuteAlways]
public class CameraFollow : MonoBehaviour
{
    public Transform carTransform; // Машина
    public float rotationSpeed = 3f; // Чувствительность поворота
    public float distance = 5f; // Расстояние до машины
    public float height = 2f; // Высота камеры
    public float smoothTime = 0.1f; // Сглаживание позиции
    public float rotationReturnSpeed = 2f; // Скорость возврата камеры
    public float returnDelay = 2f; // Задержка перед возвратом камеры (в секундах)

    [Header("Mobile Controls")]
    public RectTransform touchArea; // Прозрачный спрайт для управления камерой на мобильных устройствах

    private Vector2 rotation = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private bool isRotating = false;
    private float timeSinceLastInput = 0f;

    void Start()
    {
        rotation.x = carTransform ? carTransform.eulerAngles.y : 0;
        rotation.y = 20f;

        // Скрываем курсор при старте и включаем управление
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateCameraPosition();
        }

        // Показываем курсор при нажатии Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Скрываем курсор и включаем управление при нажатии правой кнопки мыши
        if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
        bool userInput = false;

        // Управление для ПК (только если курсор скрыт)
        if (!Cursor.visible) // Проверяем, скрыт ли курсор
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
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

            // Проверяем, находится ли касание внутри области touchArea
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touchPos))
            {
                rotation.x += touch.deltaPosition.x * 0.1f;
                rotation.y -= touch.deltaPosition.y * 0.1f;
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

            // Плавно возвращаем rotation.x к целевому значению
            rotation.x = Mathf.LerpAngle(rotation.x, targetRotationX, Time.fixedDeltaTime * rotationReturnSpeed);
        }
    }

    void UpdateCameraPosition()
    {
        if (!carTransform) return;

        Quaternion rotationQuat = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 targetPosition = carTransform.position - (rotationQuat * Vector3.forward * distance) + (Vector3.up * height);
        transform.position = Application.isPlaying ? Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime) : targetPosition;

        transform.LookAt(carTransform.position);
    }

    public void TeleportCamera()
    {
        rotation.x = carTransform.eulerAngles.y;
        transform.position = carTransform.position - (Quaternion.Euler(rotation.y, rotation.x, 0) * Vector3.forward * distance) + (Vector3.up * height);
        transform.LookAt(carTransform.position);
    }

    void OnValidate()
    {
        UpdateCameraPosition();
    }
}