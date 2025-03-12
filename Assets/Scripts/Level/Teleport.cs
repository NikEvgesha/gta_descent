using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour
{
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private bool _onInertion = true;
    [SerializeField] private AudioSource _sound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarTeleport>() != null)
        {
            if (_onInertion)
            {
                _sound.Play();
                GameManager.Instance.LevelWin?.Invoke();
            }
            UseTeleport(other.gameObject.GetComponent<CarTeleport>());
        }
    }

    private void Awake()
    {
        _spawnPoint = FindAnyObjectByType<SpawnPoint>();
    }

    private IEnumerator FadeSequence(CarTeleport car)
    {
        if (Fade.Instance != null)
        {
            yield return Fade.Instance.FadeIn();
            AfterFade(car);
            yield return Fade.Instance.FadeOut();
        }
        else
        {
            AfterFade(car);
            yield return null;
        }
    }

    private void AfterFade(CarTeleport car)
    {
        car.Teleport(_spawnPoint.GetPointToSpawn(), false);
    }
    public void UseTeleport(CarTeleport car)
    {
        StartCoroutine(FadeSequence(car));
    }
}
