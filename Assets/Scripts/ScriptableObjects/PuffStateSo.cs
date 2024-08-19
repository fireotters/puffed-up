using System;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "PuffStateSo", menuName = "FireOtters/PuffStateSo", order = 0)]
    public class PuffStateSo : ScriptableObject
    {
        PuffStateHandler.State state;
        public PuffStateHandler.State State => state;

        public void SetPuffState(PuffStateHandler.State newState)
        {
            if (state != newState)
            {
                state = newState;
                OnValueChanged?.Invoke(newState);
            }
        }

        public event Action<PuffStateHandler.State> OnValueChanged;
    }
}