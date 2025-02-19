using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private SpawnPoint spawnPoint;
    [SerializeField] private bool onInertion = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PrometeoCarController>() != null)
        {
            other.gameObject.GetComponent<PrometeoCarController>().TeleportCar(spawnPoint.GetPointToSpawn(), onInertion);
        }
    }
    private void TeleportOnStart(GameObject car)
    {
        car.transform.position = spawnPoint.GetPointToSpawn().position;
        car.transform.rotation = spawnPoint.GetPointToSpawn().rotation;
    }
}
