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
        if (!_useEffects || !_physics.IsOnGround) // Добавляем проверку IsOnGround
        {
            // Отключаем эффекты, если машина в воздухе
            if (_rearLeftParticleSystem != null) _rearLeftParticleSystem.Stop();
            if (_rearRightParticleSystem != null) _rearRightParticleSystem.Stop();
            if (_rearLeftTireSkid != null) _rearLeftTireSkid.emitting = false;
            if (_rearRightTireSkid != null) _rearRightTireSkid.emitting = false;
            return;
        }

        bool shouldEmitParticles = _physics.IsDrifting;
        bool shouldEmitTrails = _physics.IsTractionLocked || (Mathf.Abs(_physics.LocalVelocityX) > 5f && Mathf.Abs(_physics.CarSpeed) > 12f);

        if (shouldEmitParticles)
        {
            if (_rearLeftParticleSystem != null) _rearLeftParticleSystem.Play();
            if (_rearRightParticleSystem != null) _rearRightParticleSystem.Play();
        }
        else
        {
            if (_rearLeftParticleSystem != null) _rearLeftParticleSystem.Stop();
            if (_rearRightParticleSystem != null) _rearRightParticleSystem.Stop();
        }

        if (_rearLeftTireSkid != null) _rearLeftTireSkid.emitting = shouldEmitTrails;
        if (_rearRightTireSkid != null) _rearRightTireSkid.emitting = shouldEmitTrails;
    }
}