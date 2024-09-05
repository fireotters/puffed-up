using Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private Rigidbody2D _rb;
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        SignalBus<SignalBoxesSwitchToContinuousRbDetection>.Subscribe(SwitchToContinuousRbDetection).AddTo(_disposables);
    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }

    private void SwitchToContinuousRbDetection(SignalBoxesSwitchToContinuousRbDetection signal)
    {
        if (signal.ContinuousMode)
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        else
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }
}
