using UnityEngine;
using System.Collections.Generic;
using System;

// Интерфейс для рекламных провайдеров
public abstract class AdsProvider : MonoBehaviour
{
    public abstract void Initialize(); // Инициализация провайдера
    public abstract bool IsRewardedAdReady(); // Проверка готовности rewarded-рекламы
    public abstract void ShowRewardedAd(string rewardId, Action<bool> onComplete); // Показ rewarded-рекламы с коллбэком
    public abstract void ShowInterstitialAd(); // Показ interstitial-рекламы
}

// Главный менеджер рекламы
public class AdsManager : MonoBehaviour
{
    private static AdsManager _instance;
    public static AdsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AdsManager");
                _instance = go.AddComponent<AdsManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [SerializeField] private List<AdsProvider> adsProviders = new List<AdsProvider>(); // Список активных провайдеров

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        //DontDestroyOnLoad(gameObject);

        // Инициализация всех провайдеров
        InitializeProviders();
    }

    private void InitializeProviders()
    {
        foreach (var provider in adsProviders)
        {
            provider.Initialize();
            Debug.Log($"Initialized ads provider: {provider.GetType().Name}");
        }
    }

    // Проверка готовности rewarded-рекламы
    public bool IsRewardedAdReady()
    {
        if (adsProviders.Count == 0)
        {
            Debug.LogWarning("No ads providers configured!");
            return false;
        }

        foreach (var provider in adsProviders)
        {
            if (provider.IsRewardedAdReady())
            {
                return true;
            }
        }
        return false;
    }

    // Показ rewarded-рекламы через первый готовый провайдер
    public void ShowRewardedAd(string rewardId, Action<bool> onComplete)
    {
        if (adsProviders.Count == 0)
        {
            Debug.LogWarning("No ads providers configured!");
            onComplete?.Invoke(false);
            return;
        }

        foreach (var provider in adsProviders)
        {
            if (provider.IsRewardedAdReady())
            {
                provider.ShowRewardedAd(rewardId, onComplete);
                return;
            }
        }
        Debug.LogWarning("No rewarded ads available!");
        onComplete?.Invoke(false);
    }

    // Показ interstitial-рекламы через первый доступный провайдер
    public void ShowInterstitialAd()
    {
        if (adsProviders.Count == 0)
        {
            Debug.LogWarning("No ads providers configured!");
            return;
        }

        foreach (var provider in adsProviders)
        {
            provider.ShowInterstitialAd();
            return;
        }
        Debug.LogWarning("No interstitial ads available!");
    }

    // Метод для добавления провайдера в рантайме
    public void AddProvider(AdsProvider provider)
    {
        if (!adsProviders.Contains(provider))
        {
            adsProviders.Add(provider);
            provider.Initialize();
        }
    }
}

// Отладочный провайдер
public class DebugAdsProvider : AdsProvider
{
    public override void Initialize()
    {
        Debug.Log("Debug Ads initialized");
    }

    public override bool IsRewardedAdReady()
    {
        return true; // Всегда готов для отладки
    }

    public override void ShowRewardedAd(string rewardId, Action<bool> onComplete)
    {
        Debug.Log($"Debug Rewarded Ad shown with ID: {rewardId}");
        onComplete?.Invoke(true); // Симулируем успешное завершение
    }

    public override void ShowInterstitialAd()
    {
        Debug.Log("Debug Interstitial Ad shown");
    }
}