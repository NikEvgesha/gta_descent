using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private PrometeoCarController _car;
    private void Awake()
    {
        _car = FindAnyObjectByType<PrometeoCarController>();
    }
    public Transform GetPointToSpawn()
    {
        return transform;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            _car.TeleportCar(GetPointToSpawn());
        }
    }
}
