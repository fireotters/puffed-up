using Cysharp.Threading.Tasks;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth;
    ICustomPhysics _rb;
    [SerializeField] float recoilImpulseForce = 2.5f;
    public int currentHealth { get; private set; }

    [Header("Invulnerability Frames")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] AnimationCurve flashCurve;
    [SerializeField] private int numOfFlashes;
    bool isInvulnerable = false;
    private SpriteRenderer _spr;
    [SerializeField] private SpriteRenderer overlaySpriteRenderer;

    private void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
        _rb = GetComponent<ICustomPhysics>();
    }

    public void Damage(int damage, Vector2 recoilDirection)
    {
        if (isInvulnerable)
            return;

        isInvulnerable = true;

        // Delayed call, careful, don't call twice in a row without cancelling the previous one
        // Here as isInvulnerable is set to true and we have the guard close at the beggining, that won't happen
        // destroyCancellationToken is a monobehaviour generated token that cancels on destroy
        this.DelayedCall(iFrameDuration, this.destroyCancellationToken, () => isInvulnerable = false).Forget();

        this.ExecuteOverDuration(iFrameDuration, this.destroyCancellationToken, normalizedDuration =>
        {
            Color color = overlaySpriteRenderer.color;
            color.a = flashCurve.Evaluate(normalizedDuration);
            overlaySpriteRenderer.color = color;
        }).Forget();

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            print("Died-ed");
        }
        else
        {
            _rb.AddImpulse(recoilDirection * this.recoilImpulseForce);
            print("Owie I took damage: " + damage.ToString());
        }
    }
}
