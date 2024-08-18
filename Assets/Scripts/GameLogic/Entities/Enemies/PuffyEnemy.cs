using System;
using UnityEngine;

namespace GameLogic.Entities.Enemies
{
    public class PuffyEnemy : MonoBehaviour
    {
        private PuffStateHandler _enemyPuffStateHandler;

        private void Start()
        {
            _enemyPuffStateHandler = GetComponent<PuffStateHandler>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
            {
                _enemyPuffStateHandler.SetPuffed();
            }
        }
    }
}
