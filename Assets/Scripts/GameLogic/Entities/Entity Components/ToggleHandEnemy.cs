using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHandEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<HealthHandler>(out HealthHandler hh))
        {
            enemy.SetActive(true);
        }
    }
}
