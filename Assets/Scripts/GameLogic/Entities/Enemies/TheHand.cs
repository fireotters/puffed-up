using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;
using System;
using FMODUnity;
using Signals;

public class TheHand : MonoBehaviour
{
    Player player;
    Rigidbody2D _rb;
    [SerializeField][Range(0, 100)] private float minSpeed;
    [SerializeField][Range(0, 100)] private float maxSpeed;
    [SerializeField][Range(0, 100)] private float minDistance;
    [SerializeField][Range(0, 100)] private float maxDistance;
    private Animator _animator;
    [SerializeField] private StudioEventEmitter _sndPlrCaught, _stageLevel;
    private bool playerIsCaught = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        player = FindAnyObjectByType<Player>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
            return;
        var direction = (Vector2)player.transform.position - (Vector2)transform.position;
        if (playerIsCaught)
            direction = Vector2.up;

        var currentSpeed = GetSpeed();
        
        _rb.AddForce(direction.normalized * currentSpeed);

        if (currentSpeed <= minSpeed + 3) // i dont care one bit, hardcoding time fuckers
        {
            _stageLevel.SetParameter("In_Danger", 1);
        }
        else
        {
            _stageLevel.SetParameter("In_Danger", 0);
        }
        
        // Rotate sprite
        if (direction.x > 0f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
        else if (direction.x < -0f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        }
    }

    float GetSpeed()
    {
        if (playerIsCaught)
            return 15f;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        float mappedNormalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
        return Mathf.Lerp(minSpeed, maxSpeed, mappedNormalizedDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<HealthHandler>(out HealthHandler hh))
        { 
            hh.GotCaught();
            Invoke(nameof(CaughtPlayer), 0.15f); // 0.15 is duration of 'big to small' Puffy anim
        }
    }

    private void CaughtPlayer()
    {
        if (!playerIsCaught)
        {
            playerIsCaught = true;
            _animator.Play("GotYou");
            _sndPlrCaught.Play();
            Invoke(nameof(CaughtPlayer2), 2f);
        }
    }

    private void CaughtPlayer2()
    {
        SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { result = GameEndCondition.Loss });
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
            if (player == null)
                return;

            Vector3 direction = (Vector2)player.transform.position - (Vector2)transform.position;
            Gizmos.DrawLine(transform.position + direction.normalized * minDistance, transform.position + direction.normalized * maxDistance);
            Gizmos.DrawWireSphere(transform.position + direction.normalized * minDistance, 1.0f);
            Gizmos.DrawWireSphere(transform.position + direction.normalized * maxDistance, 1.0f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + direction.normalized * GetSpeed());
        #endif
    }
}
