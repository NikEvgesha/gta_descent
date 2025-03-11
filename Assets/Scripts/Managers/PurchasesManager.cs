using UnityEngine;
using System;

// Интерфейс для провайдеров покупок
public abstract class PurchasesProvider : MonoBehaviour
{
    public abstract void Initialize(); // Инициализация провайдера
    public abstract void BuyPurchase(string purchaseId, Action<bool> onComplete); // Вызов покупки с коллбэком
    public abstract void ConsumePendingPurchases(); // Обработка необработанных покупок
    public abstract PurchaseData GetPurchaseData(string purchaseId); // Получение данных о покупке
}

// Данные о покупке (универсальная структура)
public class PurchaseData
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Price { get; private set; }

    public PurchaseData(string id, string title, string description, string price)
    {
        Id = id;
        Title = title;
        Description = description;
        Price = price;
    }
}

// Главный менеджер покупок
public class PurchasesManager : MonoBehaviour
{
    private static PurchasesManager _instance;
    public static PurchasesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("PurchasesManager");
                _instance = go.AddComponent<PurchasesManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [SerializeField] private MonoBehaviour activeProvider; // Активный провайдер в инспекторе
    private PurchasesProvider provider;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        //DontDestroyOnLoad(gameObject);

        // Проверка и инициализация провайдера
        if (activeProvider == null || !activeProvider.TryGetComponent(out provider))
        {
            Debug.LogError("No valid purchases provider assigned!");
            return;
        }

        provider.Initialize();
        provider.ConsumePendingPurchases(); // Обрабатываем необработанные покупки при старте
    }

    // Вызов покупки
    public void BuyPurchase(string purchaseId, Action<bool> onComplete)
    {
        if (provider == null)
        {
            Debug.LogError("Purchases provider not initialized!");
            onComplete?.Invoke(false);
            return;
        }

        provider.BuyPurchase(purchaseId, onComplete);
    }

    // Получение данных о покупке
    public PurchaseData GetPurchaseData(string purchaseId)
    {
        if (provider == null)
        {
            Debug.LogError("Purchases provider not initialized!");
            return null;
        }

        return provider.GetPurchaseData(purchaseId);
    }

    // Установка нового провайдера в рантайме (опционально)
    public void SetProvider(PurchasesProvider newProvider)
    {
        provider = newProvider;
        provider.Initialize();
        provider.ConsumePendingPurchases();
    }
}

// Отладочный провайдер
public class DebugPurchasesProvider :  PurchasesProvider
{
    public override void Initialize()
    {
        Debug.Log("Debug Purchases initialized");
    }

    public override void BuyPurchase(string purchaseId, Action<bool> onComplete)
    {
        Debug.Log($"Debug Purchase requested: {purchaseId}");
        onComplete?.Invoke(true); // Симулируем успешную покупку
    }

    public override void ConsumePendingPurchases()
    {
        Debug.Log("Debug Consuming pending purchases (none)");
    }

    public override PurchaseData GetPurchaseData(string purchaseId)
    {
        return new PurchaseData(purchaseId, "Test Item", "A debug purchase", "1.99 USD");
    }
}