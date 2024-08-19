using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMassOnPuff : MonoBehaviour
{
    [SerializeField] PuffStateSo puffStateSO;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] float massWhenDeflated = 20.0f;
    [SerializeField] float massWhenPuffed = 5.0f;

    public float Mass => puffStateSO.State == PuffStateHandler.State.Puffed ? massWhenPuffed : massWhenDeflated;

    private void Start()
    {
        puffStateSO.OnValueChanged += OnPuffStateChanged;
        OnPuffStateChanged(puffStateSO.State);
    }

    private void OnDestroy()
    {
        puffStateSO.OnValueChanged -= OnPuffStateChanged;
    }

    void OnPuffStateChanged(PuffStateHandler.State state)
    {
        Debug.Log($"Puff state {state}");
        rb.mass = Mass;
    }

    private void OnValidate()
    {
        if (puffStateSO != null)
            GetComponent<Rigidbody2D>().mass = Mass;
    }
}
