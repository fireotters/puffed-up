using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PuffStateHandler : MonoBehaviour
{
    public enum State { Deflated, Puffed };

    [SerializeField] private State state = State.Deflated;
    [Range(0.1f, 100.0f)][SerializeField] private float massWhenPuffed;
    [Range(0.1f, 100.0f)][SerializeField] private float massWhenDeflated;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool IsPuffed => state == State.Puffed;
    public bool IsDeflated => state == State.Deflated;
    public State PuffState => state;

    float Mass => state == State.Puffed ? massWhenPuffed : massWhenDeflated;

    public void SetState(State state)
    {
        this.state = state;
        rb.mass = Mass;
    }

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = Mass;
    }
}
