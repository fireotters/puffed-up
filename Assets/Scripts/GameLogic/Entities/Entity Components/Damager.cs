using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] int damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<HealthHandler>(out HealthHandler hh))
        {
            Vector2 recoilDirection = ((Vector2)collision.transform.position - collision.contacts[0].point).normalized;
            hh.Damage(damage, recoilDirection);
        }
    }
}
