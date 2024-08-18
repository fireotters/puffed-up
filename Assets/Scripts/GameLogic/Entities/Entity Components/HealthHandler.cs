using Cysharp.Threading.Tasks;
using GameLogic;
using UnityEngine;
using UnityEngine.Events;

public class HealthHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth;
    ICustomPhysics _rb;
    [SerializeField] float recoilImpulseForce = 2.5f;
    public int currentHealth { get; private set; }
    [SerializeField] UnityEvent onHurt, onDeath;
    private bool dead = false;

    [Header("Invulnerability Frames")]
    [SerializeField] private float iFrameDuration;
    [SerializeField] AnimationCurve flashCurve;
    [SerializeField] private int numOfFlashes;
    bool isInvulnerable = false;
    [SerializeField] private SpriteRenderer _spr;
    [SerializeField] private Color _colorWhenDamaged;

    private void Start()
    {
        currentHealth = startingHealth;
        _rb = GetComponent<ICustomPhysics>();
    }

    public bool IsAlive => dead == false;

    public void Damage(int damage, Vector2 recoilDirection)
    {
        if (isInvulnerable)
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

    private void EndInvuln()
    {
        isInvulnerable = false;
        Color color = new(1, 1, 1, 1); // Reset to normal color
        _spr.color = color;
    }
}
