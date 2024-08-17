using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "PuffStateSo", menuName = "FireOtters/PuffStateSo", order = 0)]
    public class PuffStateSo : ScriptableObject
    {
        public PuffStateHandler.State state;
        public PuffStateHandler.State State => state;

        public void SetPuffState(PuffStateHandler.State newState)
        {
            state = newState;
        }
    }
}