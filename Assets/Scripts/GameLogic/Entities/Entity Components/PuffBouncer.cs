using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffBouncer : MonoBehaviour
{
    [SerializeField] float forceWhenPuffed = 10.0f;
    [SerializeField] float forceWhenDeflated = 4.0f;

    float Force => _stateHandler.IsPuffed ? forceWhenPuffed : forceWhenDeflated;

    ICustomPhysics _rb;
    PuffStateHandler _stateHandler;

    private void Start()
    {
        _rb = GetComponent<ICustomPhysics>();
        _stateHandler = GetComponent<PuffStateHandler>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Bouncable bouncable))
        {
            Vector2 recoilDirection = ((Vector2)collision.transform.position - collision.contacts[0].normal).normalized;
            _rb.AddImpulse(recoilDirection * -Force);
        }
    }
}
