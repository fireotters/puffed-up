using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;
using System;
using FMODUnity;

public class TheHand : MonoBehaviour
{
    Player player;
    Rigidbody2D _rb;
    [SerializeField][Range(0, 100)] private float minSpeed;
    [SerializeField][Range(0, 100)] private float maxSpeed;
    [SerializeField][Range(0, 100)] private float minDistance;
    [SerializeField][Range(0, 100)] private float maxDistance;
    private Animator _animator;
    [SerializeField] private StudioEventEmitter _sndPlrCaught;
    private bool _hasPlayedCaughtSfx = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        player = FindAnyObjectByType<Player>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var direction = (Vector2)player.transform.position - (Vector2)transform.position;
        _rb.AddForce(direction.normalized * GetSpeed());
    }

    float GetSpeed()
    {
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
        _animator.Play("GotYou");
        if (!_hasPlayedCaughtSfx)
            _sndPlrCaught.Play();
        _hasPlayedCaughtSfx = true;
    }

    private void OnDrawGizmos()
    {
        if (player == null)
            player = FindAnyObjectByType<Player>();

        Vector3 direction = (Vector2)player.transform.position - (Vector2)transform.position;
        Gizmos.DrawLine(transform.position + direction.normalized * minDistance, transform.position + direction.normalized * maxDistance);
        Gizmos.DrawWireSphere(transform.position + direction.normalized * minDistance, 1.0f);
        Gizmos.DrawWireSphere(transform.position + direction.normalized * maxDistance, 1.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + direction.normalized * GetSpeed());
    }
}
