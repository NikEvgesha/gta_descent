using UnityEngine;

[ExecuteAlways] // Позволяет скрипту работать в редакторе
public class CameraFollow : MonoBehaviour
{
    public Transform carTransform; // Машина
    public float rotationSpeed = 3f; // Чувствительность поворота
    public float distance = 5f; // Расстояние до машины
    public float height = 2f; // Высота камеры
    public float smoothTime = 0.1f; // Сглаживание

    private Vector2 rotation = Vector2.zero;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        rotation.x = carTransform ? carTransform.eulerAngles.y : 0;
        rotation.y = 20f;
    }

    void Update()
    {
        // В редакторе обновляем камеру без запуска игры
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
        if (Input.GetMouseButton(1)) // ПК: Поворот камеры правой кнопкой мыши
        {
            rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
            rotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;
            rotation.y = Mathf.Clamp(rotation.y, 5f, 60f);
        }

        if (Input.touchCount > 0) // Мобильное управление (правая часть экрана)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > Screen.width / 2)
            {
                rotation.x += touch.deltaPosition.x * 0.1f;
                rotation.y -= touch.deltaPosition.y * 0.1f;
                rotation.y = Mathf.Clamp(rotation.y, 5f, 60f);
            }
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
