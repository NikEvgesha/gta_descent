using UnityEngine;

public class CarTeleport : MonoBehaviour
{
    private Rigidbody _rb;
    private SpawnPoint _spawnPoint;
    private bool _wasSpawnPressed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _spawnPoint = FindAnyObjectByType<SpawnPoint>();
    }

    public void HandleTeleport(bool spawnPressed)
    {
        if (spawnPressed && !_wasSpawnPressed)
        {
            Teleport(_spawnPoint.GetPointToSpawn());
        }
        _wasSpawnPressed = spawnPressed;
    }

    public void Teleport(Transform spawn, bool preserveInertia = false)
    {
        Vector3 velocity = preserveInertia ? _rb.velocity : Vector3.zero;
        Vector3 angularVelocity = preserveInertia ? _rb.angularVelocity : Vector3.zero;

        _rb.position = spawn.position;
        _rb.rotation = spawn.rotation;

        _rb.velocity = spawn.forward * velocity.magnitude;
        _rb.angularVelocity = angularVelocity;
    }
}