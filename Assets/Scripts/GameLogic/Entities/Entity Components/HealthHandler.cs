using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth;
    public int currentHealth { get; private set; }

    [Header("Invulnerability Frames")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] private int numOfFlashes;
    private SpriteRenderer _spr;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void Damage(int _damage)
    {
        currentHealth -= _damage;
        if (currentHealth < startingHealth)
        {
            print("Died-ed");
        }
        else
        {
            print("Owie I took damage: " + _damage.ToString());
        }
    }
}
