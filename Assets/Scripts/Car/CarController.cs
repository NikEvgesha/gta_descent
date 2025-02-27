using UnityEngine;

[RequireComponent(typeof(CarPhysics), typeof(CarEffects), typeof(CarAudio))]
[RequireComponent(typeof(CarUI), typeof(CarInput), typeof(CarTeleport))]
public class CarController : MonoBehaviour
{
    private CarPhysics _physics;
    private CarEffects _effects;
    private CarAudio _audio;
    private CarUI _ui;
    private CarInput _input;
    private CarTeleport _teleport;

    private void Awake()
    {
        _physics = GetComponent<CarPhysics>();
        _effects = GetComponent<CarEffects>();
        _audio = GetComponent<CarAudio>();
        _ui = GetComponent<CarUI>();
        _input = GetComponent<CarInput>();
        _teleport = GetComponent<CarTeleport>();
    }

    private void Update()
    {
        // Оставляем только нефизические обновления в Update
        _effects.UpdateEffects();
        _audio.UpdateSounds();
        _ui.UpdateUI();
        _teleport.HandleTeleport(_input.SpawnPressed);
    }

    private void FixedUpdate()
    {
        // Переносим физику в FixedUpdate
        _physics.ApplyAcceleration(_input.ThrottlePressed, _input.ReversePressed);
        _physics.ApplySteering(_input.TurnLeftPressed, _input.TurnRightPressed);
        _physics.ApplyHandbrake(_input.HandbrakePressed);
    }
}