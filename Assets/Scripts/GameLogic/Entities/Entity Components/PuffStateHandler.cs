using ExtensionsFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using GameLogic;
using UnityEngine.Events;
using Signals;

public class PuffStateHandler : MonoBehaviour
{
    public enum State { Deflated, Puffed };

    [Header("PuffState")]
    [SerializeField] private State state = State.Deflated;
    [SerializeField] private PuffStateSo playerPuffStateSo;

    [Header("Physics")]
    [Range(0.1f, 30.0f)][SerializeField] private float massWhenPuffed;
    [Range(0.1f, 30.0f)][SerializeField] private float massWhenDeflated;
    private Rigidbody2D rb;

    [Header("Cooldowns")]
    private float lastDeflateTime, waitFromDeflateToNextInflate = 2;

    [Header("Misc")]
    [SerializeField] private float scaleWhenInflated;
    CancellationTokenSource cancellationToken;
    [SerializeField] GameStateSo gameState;

    [SerializeField] UnityEvent OnPuffed;
    [SerializeField] UnityEvent OnDeflated;
    [SerializeField] private bool isPlayer = false;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        SetState(state);
        if (isPlayer)
            lastDeflateTime = Time.time - waitFromDeflateToNextInflate;
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
        rb.mass = Mass;
        if (isPlayer)
        {
            playerPuffStateSo.SetPuffState(newState);
        }

        if (state == State.Puffed)
            OnBecomePuffed();
        else
            OnBecomeDeflated();
    }

    private void OnBecomePuffed()
    {
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        this.LerpScale(Vector2.one * scaleWhenInflated, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
        if (isPlayer)
        {
            OnPuffed?.Invoke();
        }
    }

    private void OnBecomeDeflated()
    {
        if (isPlayer)
        {
            gameState.LastPlayerPuffTime.Value = Time.time;
            gameState.PlayerPuffWaitTime.Value = waitFromDeflateToNextInflate;
            lastDeflateTime = Time.time;
            OnDeflated?.Invoke();
        }
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        this.LerpScale(Vector2.one, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
    }

    // Used in the unity event
    public void SetPuffed() => SetState(State.Puffed);

    public void SetDeflated() => SetState(State.Deflated);
}
