using System;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "GameStateSo", menuName = "FireOtters/GameStateSo", order = 0)]
    public class GameStateSo : ScriptableObject
    {
        public ReactiveData<int> Shells;
        public ReactiveData<int> Pearls;
        public ReactiveData<int> Health;
        public ReactiveData<int> PuffTimeCurrent, PuffTimeMax;

        private void OnEnable()
        {
            Shells.Value = 0;
            Pearls.Value = 0;
        }
    }
}