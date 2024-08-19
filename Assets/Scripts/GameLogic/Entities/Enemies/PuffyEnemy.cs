using System;
using UnityEngine;

namespace GameLogic.Entities.Enemies
{
    public class PuffyEnemy : MonoBehaviour
    {
        private PuffStateHandler _enemyPuffStateHandler;
        private Animator _animator;

        private void Start()
        {
            _enemyPuffStateHandler = GetComponent<PuffStateHandler>();
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
            {
                _animator.Play("Scare");
                _enemyPuffStateHandler.SetPuffed();
            }
        }
    }
}
