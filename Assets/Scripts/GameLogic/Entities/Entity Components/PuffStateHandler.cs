using ExtensionsFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using GameLogic;

public class PuffStateHandler : MonoBehaviour
{
    public enum State { Deflated, Puffed };

    [Header("PuffState")]
    [SerializeField] private State state = State.Deflated;
    [SerializeField] private PuffStateSo playerPuffStateSo;

    [Header("Physics")]
    [Range(0.1f, 100.0f)][SerializeField] private float massWhenPuffed;
    [Range(0.1f, 100.0f)][SerializeField] private float massWhenDeflated;
    private Rigidbody2D rb;
    private CircleCollider2D _collider;

    [Header("Cooldowns")]
    private float lastDeflateTime, waitFromDeflateToNextInflate = 2;

    [Header("Misc")]
    [SerializeField] private float scaleWhenInflated;
    CancellationTokenSource cancellationToken;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        SetState(state);
        lastDeflateTime = Time.time;
    }

    private void OnDestroy()
    {
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
    }

    public bool IsPuffed => state == State.Puffed;
    public bool IsDeflated => state == State.Deflated;
    public State PuffState => state;
    public bool CanInflateAgainYet => lastDeflateTime < Time.time - waitFromDeflateToNextInflate;

    float Mass => state == State.Puffed ? massWhenPuffed : massWhenDeflated;

    public void SetState(State newState)
    {
        state = newState;
        playerPuffStateSo.SetPuffState(newState);
        rb.mass = Mass;

        if (state == State.Puffed)
            OnBecomePuffed();
        else
            OnBecomeDeflated();
    }

    private void OnBecomePuffed()
    {
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        this.LerpScale(Vector2.one * scaleWhenInflated, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
    }

    private void OnBecomeDeflated()
    {
        lastDeflateTime = Time.time;
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        this.LerpScale(Vector2.one, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
    }

    // Used in the unity event
    public void SetPuffed() => SetState(State.Puffed);

    public void SetDeflated() => SetState(State.Deflated);

    public void OnDeath()
    {
        _collider.radius = 0.076f; // Adjust collider size, so that the sprite will lie on the floor instead of floating
    }
}
