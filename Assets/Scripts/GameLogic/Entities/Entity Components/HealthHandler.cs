using Cysharp.Threading.Tasks;
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
    bool isInvulnerable = false;
    private SpriteRenderer _spr;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void Damage(int damage)
    {
        if (isInvulnerable)
            return;

        isInvulnerable = true;

        // Delayed call, careful, don't call twice in a row without cancelling the previous one
        // Here as isInvulnerable is set to true and we have the guard close at the beggining, that won't happen
        // destroyCancellationToken is a monobehaviour generated token that cancels on destroy
        this.DelayedCall(iFrameDuration, this.destroyCancellationToken, () => isInvulnerable = false).Forget();

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            print("Died-ed");
        }
        else
        {
            print("Owie I took damage: " + damage.ToString());
        }
    }
}
