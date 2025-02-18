using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] SpawnPoint spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PrometeoCarController>() != null)
        {
            other.gameObject.GetComponent<PrometeoCarController>().TeleportCar(spawnPoint.GetPointToSpawn());
        }
    }
    private void TeleportOnStart(GameObject car)
    {
        car.transform.position = spawnPoint.GetPointToSpawn().position;
        car.transform.rotation = spawnPoint.GetPointToSpawn().rotation;
    }
}
