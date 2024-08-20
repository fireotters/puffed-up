using GameLogic;
using Signals;
using System;
using UnityEngine;

namespace Editor_Related
{
    public class PlayerEnd : MonoBehaviour
    {
        
        private SpriteRenderer _sprite;

        void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _sprite.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Make sure only gameObjects tagged as Player can set off LevelFinished
            if (other.gameObject.TryGetComponent(out Player player))
            {
                SignalBus<SignalGameEnded>.Fire(new SignalGameEnded { result = GameEndCondition.Win });
            }
        }
    }
}


