using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private bool _useSounds = false;
    [SerializeField] private AudioSource _carEngineSound;
    [SerializeField] private AudioSource _tireScreechSound;

    private CarPhysics _physics;
    private Rigidbody _rb;
    private float _initialEnginePitch;

    private void Awake()
    {
        _physics = GetComponent<CarPhysics>();
        _rb = GetComponent<Rigidbody>();
        if (_carEngineSound != null) _initialEnginePitch = _carEngineSound.pitch;
    }

    public void UpdateSounds()
    {
        if (!_useSounds) return;

        if (_carEngineSound != null)
        {
            _carEngineSound.pitch = _initialEnginePitch + (_rb.velocity.magnitude / 25f);
        }

        if (_tireScreechSound != null)
        {
            bool shouldPlay = _physics.IsOnGround && // Добавляем проверку IsOnGround
                            (_physics.IsDrifting || (_physics.IsTractionLocked && Mathf.Abs(_physics.CarSpeed) > 12f));
            if (shouldPlay && !_tireScreechSound.isPlaying)
            {
                _tireScreechSound.Play();
            }
            else if (!shouldPlay && _tireScreechSound.isPlaying)
            {
                _tireScreechSound.Stop();
            }
        }
    }
}