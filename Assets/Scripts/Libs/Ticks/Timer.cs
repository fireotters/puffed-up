using System;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [Serializable]
    public struct TimerData
    {
        public bool startOnAwake;
        public bool oneShot;
        [Min(0.001f)] public float duration;
    }

    [SerializeField] TimerData timerData = new TimerData { startOnAwake = true, oneShot = false, duration = 1f };
    TimerData copy;

    public UnityEvent OnTimerEnd;

    float timer = 0;

    public bool IsDone => timer >= timerData.duration;
    public bool IsPaused { get; protected set; } = true;

    public float TimeLeft => timerData.duration - timer;

    public float Duration
    {
        get => timerData.duration;
        protected set => timerData.duration = value;
    }

    protected virtual void Awake()
    {
        copy = timerData;

        if (timerData.startOnAwake)
        {
            StartTimer(timerData.duration);
            IsPaused = false;
        }
    }

    public void StartTimer(float waitTime)
    {
        print("Starting timer for " + waitTime);
        timerData.duration = waitTime;
        timer = 0;
    }

    // Timer will be paused until resumed. StartTimer won't resume the timer.
    public void Pause()
    {
        IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }

    private void Update()
    {
        if (IsPaused)
            return;

        timer += Time.deltaTime;

        if (IsDone)
        {
            OnTimerEnded();
        }
    }

    protected virtual void OnTimerEnded()
    {
        if(timerData.oneShot)
            IsPaused = true;
        else
            timer -= timerData.duration;

        OnTimerEnd?.Invoke();
    }

    private void OnDestroy()
    {
        OnTimerEnd.RemoveAllListeners();
    }
}
