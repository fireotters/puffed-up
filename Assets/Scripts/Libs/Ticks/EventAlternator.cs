using UnityEngine;
using UnityEngine.Events;

public class EventAlternator : Timer
{
    public UnityEvent OnTimerA;
    public UnityEvent OnTimerB;

    private bool isEventA = true;
    [SerializeField] float alternationDuration = 1f;

    float initialDuration;

    protected override void Awake()
    {
        initialDuration = Duration;
        base.Awake();
    }

    protected override void OnTimerEnded()
    {
        base.OnTimerEnded();
        
        if (isEventA)
        {
            OnTimerA?.Invoke();
            Duration = alternationDuration;
        }
        else
        {
            OnTimerB?.Invoke();
            Duration = initialDuration;
        }

        isEventA = !isEventA;
    }
}
