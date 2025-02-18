using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] SpawnPoint spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PrometeoCarController>() != null)
        {
            TeleportOnStart(other.gameObject);
        }
    }
    private void TeleportOnStart(GameObject car)
    {
        car.transform.position = spawnPoint.GetPointToSpawn().position;
        car.transform.rotation = spawnPoint.GetPointToSpawn().rotation;
    }
}
