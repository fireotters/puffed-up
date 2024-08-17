using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleStateMachine : MonoBehaviour
{
    public struct Transition
    {
        public Func<bool> condition;
        public State target;
    }

    public struct State
    {
        public Action<SimpleStateMachine> onStateEnter;
        public Action<SimpleStateMachine> onStateExit;
        public Action<SimpleStateMachine>  onStateStay;
        public List<Transition> transitions;

        public void OnStateExit(SimpleStateMachine ssm) => onStateExit?.Invoke(ssm);

        public void OnStateEnter(SimpleStateMachine ssm) => onStateEnter?.Invoke(ssm);

        public void OnStateStay(SimpleStateMachine ssm) => onStateStay?.Invoke(ssm);
    }

    State currentState;

    public void Update()
    {
        State? newState = GetNewStateFrom(currentState.transitions);
        if(newState != null)
            SetState(newState.Value);
        else
            currentState.OnStateStay(this);
    }

    State? GetNewStateFrom(List<Transition> transitions)
    {
        if(transitions == null ) return null;

        foreach(var transition in transitions)
            if(transition.condition.Invoke())
                return transition.target;

        return null;
    }

    public void SetState(State newState)
    {
        currentState.OnStateExit(this);
        newState.OnStateEnter(this);
        currentState = newState;
    }
}
