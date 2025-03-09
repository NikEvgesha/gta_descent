using UnityEngine;
using UnityEngine.UI;

public class AdaptiveUILayout : MonoBehaviour
{
    private RectTransform rectTransform;
    public float minPadding = 50f;  // Минимальный фиксированный отступ (красная зона)
    public float minWidth = 1280f;  // Минимальная ширина GemsShop
    public float minHeight = 720f;  // Минимальная высота GemsShop
    public float maxWidth = 1920f;  // Максимальная ширина GemsShop
    public float maxHeight = 1080f; // Максимальная высота GemsShop
    private float aspectRatio;       // Соотношение сторон (ширина/высота)

    // Ссылки на панели отступов
    public RectTransform leftPanel;
    public RectTransform rightPanel;
    public RectTransform topPanel;
    public RectTransform bottomPanel;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        aspectRatio = maxWidth / maxHeight; // Например, 16:9 = 1920/1080  1.777
        UpdateLayout();
    }

    void Update()
    {
        UpdateLayout();
    }

    void UpdateLayout()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Текущие размеры GemsShop
        float currentWidth = rectTransform.sizeDelta.x;
        float currentHeight = rectTransform.sizeDelta.y;

        // Вычисляем доступное пространство с учётом минимального отступа
        float availableWidth = screenWidth - (minPadding * 2);
        float availableHeight = screenHeight - (minPadding * 2);

        // Вычисляем масштабный коэффициент
        float widthScale = availableWidth / maxWidth;
        float heightScale = availableHeight / maxHeight;
        float scaleFactor = Mathf.Min(widthScale, heightScale, 1f); // Не превышаем максимальный размер

        // Применяем масштаб с сохранением пропорций
        float targetWidth = maxWidth * scaleFactor;
        float targetHeight = maxHeight * scaleFactor;

        // Ограничиваем минимальный и максимальный размер
        targetWidth = Mathf.Clamp(targetWidth, minWidth, maxWidth);
        targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);

        // Корректируем, чтобы сохранить соотношение сторон
        if (targetWidth / targetHeight > aspectRatio)
        {
            targetWidth = targetHeight * aspectRatio;
        }
        else if (targetHeight / targetWidth > 1 / aspectRatio)
        {
            targetHeight = targetWidth / aspectRatio;
        }

        // Устанавливаем новый размер GemsShop
        rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

        // Центрируем GemsShop
        rectTransform.anchoredPosition = Vector2.zero;

        // Обновляем размеры панелей
        float leftRightPadding = (screenWidth - targetWidth) / 2;    // Общий отступ слева/справа
        float topBottomPadding = (screenHeight - targetHeight) / 2;   // Общий отступ сверху/снизу

        // Минимальный отступ гарантирован, остальное — динамическое
        leftPanel.sizeDelta = new Vector2(Mathf.Max(minPadding, leftRightPadding), targetHeight);
        rightPanel.sizeDelta = new Vector2(Mathf.Max(minPadding, leftRightPadding), targetHeight);
        topPanel.sizeDelta = new Vector2(targetWidth, Mathf.Max(minPadding, topBottomPadding));
        bottomPanel.sizeDelta = new Vector2(targetWidth, Mathf.Max(minPadding, topBottomPadding));

        // Корректируем позицию панелей (если нужно)
        leftPanel.anchoredPosition = new Vector2(-targetWidth / 2 - leftPanel.sizeDelta.x / 2, 0);
        rightPanel.anchoredPosition = new Vector2(targetWidth / 2 + rightPanel.sizeDelta.x / 2, 0);
        topPanel.anchoredPosition = new Vector2(0, targetHeight / 2 + topPanel.sizeDelta.y / 2);
        bottomPanel.anchoredPosition = new Vector2(0, -targetHeight / 2 - bottomPanel.sizeDelta.y / 2);
    }
}