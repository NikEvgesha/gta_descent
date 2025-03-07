using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private bool _onInertion = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarTeleport>() != null)
        {
            if (_onInertion)
            {
                GameManager.Instance.LevelWin?.Invoke();
            }
            other.gameObject.GetComponent<CarTeleport>().Teleport(_spawnPoint.GetPointToSpawn(), _onInertion);
        }
    }
    private void Awake()
    {
        _spawnPoint = FindAnyObjectByType<SpawnPoint>();
    }
}
