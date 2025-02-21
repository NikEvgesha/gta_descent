using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private SpawnPoint _spawnPoint;
    [SerializeField] private bool _onInertion = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PrometeoCarController>() != null)
        {
            other.gameObject.GetComponent<PrometeoCarController>().TeleportCar(_spawnPoint.GetPointToSpawn(), _onInertion);
        }
    }
    private void Awake()
    {
        _spawnPoint = FindAnyObjectByType<SpawnPoint>();
    }
}
