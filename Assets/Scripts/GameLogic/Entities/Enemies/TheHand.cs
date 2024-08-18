using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;
using System;

public class TheHand : MonoBehaviour
{
    Player player;
    Rigidbody2D _rb;
    [SerializeField][Range(0, 100)] private float minSpeed;
    [SerializeField][Range(0, 100)] private float maxSpeed;
    [SerializeField][Range(0, 100)] private float minDistance;
    [SerializeField][Range(0, 100)] private float maxDistance;

    void Start()
    {
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
            Vector2 recoilDirection = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
            hh.Damage(Int16.MaxValue, recoilDirection);
        }
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
