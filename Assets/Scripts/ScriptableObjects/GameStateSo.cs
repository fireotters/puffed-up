using System;
using UnityEngine;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "GameStateSo", menuName = "FireOtters/GameStateSo", order = 0)]
    public class GameStateSo : ScriptableObject
    {
        public ReactiveData<int> Coins;

        private void OnEnable()
        {
            Coins.Value = 0;
        }
    }
}