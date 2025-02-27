using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [SerializeField] private bool _useEffects = false;
    [SerializeField] private ParticleSystem _rearLeftParticleSystem;
    [SerializeField] private ParticleSystem _rearRightParticleSystem;
    [SerializeField] private TrailRenderer _rearLeftTireSkid;
    [SerializeField] private TrailRenderer _rearRightTireSkid;

    private CarPhysics _physics;

    private void Awake()
    {
        _physics = GetComponent<CarPhysics>();
    }

    public void UpdateEffects()
    {
        if (!_useEffects || !_physics.IsOnGround) return;

        bool shouldEmitParticles = _physics.IsDrifting;
        bool shouldEmitTrails = _physics.IsTractionLocked || (Mathf.Abs(_physics.LocalVelocityX) > 5f && Mathf.Abs(_physics.CarSpeed) > 12f);

        if (shouldEmitParticles)
        {
            _rearLeftParticleSystem.Play();
            _rearRightParticleSystem.Play();
        }
        else
        {
            _rearLeftParticleSystem.Stop();
            _rearRightParticleSystem.Stop();
        }

        _rearLeftTireSkid.emitting = shouldEmitTrails;
        _rearRightTireSkid.emitting = shouldEmitTrails;
    }
}