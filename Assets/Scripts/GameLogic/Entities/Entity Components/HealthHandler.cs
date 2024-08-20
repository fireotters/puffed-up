using Cysharp.Threading.Tasks;
using GameLogic;
using Signals;
using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth;
    ICustomPhysics _rb;
    [SerializeField] float recoilImpulseForce = 2.5f;
    public int currentHealth { get; private set; }
    [SerializeField] UnityEvent onHurt, onDeath, onCaught;
    private bool dead = false;

    [Header("Invulnerability Frames")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] AnimationCurve flashCurve;
    [SerializeField] private int numOfFlashes;
    bool isInvulnerable = false;
    [SerializeField] private SpriteRenderer _spr;
    [SerializeField] private Color _colorWhenDamaged;

    [Header("Other")]
    [SerializeField] GameStateSo gameState;
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        currentHealth = startingHealth;
        gameState.Health.Value = startingHealth;
        _rb = GetComponent<ICustomPhysics>();
        SignalBus<SignalPlayerHealed>.Subscribe(Heal).AddTo(_disposables);
    }

    public bool IsAlive => dead == false;

    public void Damage(int damage, Vector2 recoilDirection)
    {
        if (isInvulnerable || dead)
            return;

        isInvulnerable = true;
        _spr.color = _colorWhenDamaged;

        // Delayed call, careful, don't call twice in a row without cancelling the previous one
        // Here as isInvulnerable is set to true and we have the guard close at the beggining, that won't happen
        // destroyCancellationToken is a monobehaviour generated token that cancels on destroy
        this.DelayedCall(iFrameDuration, this.destroyCancellationToken, () => EndInvuln()).Forget();

        this.ExecuteOverDuration(iFrameDuration, this.destroyCancellationToken, normalizedDuration =>
        {
            Color color = _spr.color;
            color.a = flashCurve.Evaluate(normalizedDuration);
            if (isInvulnerable)
                _spr.color = color;
        }).Forget();

        currentHealth -= damage;
        gameState.Health.Value = currentHealth;
        if (currentHealth <= 0)
        {
            print("Died-ed");
            dead = true;
            onDeath?.Invoke();
        }
        else
        {
            onHurt?.Invoke();
            _rb.AddImpulse(recoilDirection * this.recoilImpulseForce);
            print("Owie I took damage: " + damage.ToString());
        }
    }

    public void Heal(SignalPlayerHealed signal)
    {
        if (currentHealth < startingHealth)
        {
            currentHealth += signal.heal;
            gameState.Health.Value = currentHealth;
        }
    }

    private void EndInvuln()
    {
        isInvulnerable = false;
        Color color = new(1, 1, 1, 1); // Reset to normal color
        _spr.color = color;
    }

    public void GotCaught()
    {
        currentHealth = 0;
        gameState.Health.Value = 0;
        dead = true;
        onCaught?.Invoke();
    }
}
