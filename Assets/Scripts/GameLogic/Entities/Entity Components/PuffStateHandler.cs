using ExtensionsFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using GameLogic;

[RequireComponent(typeof(Rigidbody2D))]
public class PuffStateHandler : MonoBehaviour
{
    public enum State { Deflated, Puffed };

    [SerializeField] private State state = State.Deflated;
    [SerializeField] private PuffStateSo playerPuffStateSo;
    [Range(0.1f, 100.0f)][SerializeField] private float massWhenPuffed;
    [Range(0.1f, 100.0f)][SerializeField] private float massWhenDeflated;
    Rigidbody2D rb;
    CancellationTokenSource cancellationToken;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetState(state);
    }

    private void OnDestroy()
    {
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
    }

    public bool IsPuffed => state == State.Puffed;
    public bool IsDeflated => state == State.Deflated;
    public State PuffState => state;

    float Mass => state == State.Puffed ? massWhenPuffed : massWhenDeflated;

    public void SetState(State newState)
    {
        this.state = newState;
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
        this.LerpScale(Vector2.one * 3, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
    }

    private void OnBecomeDeflated()
    {
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        this.LerpScale(Vector2.one, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
    }

    // Used in the unity event
    public void SetPuffed() => SetState(State.Puffed);

    public void SetDeflated() => SetState(State.Deflated);

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = Mass;
    }
}
