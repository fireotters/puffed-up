using System;
using UnityEngine;

namespace GameLogic.Entities.Enemies
{
    public class SwordFishEnemy : MonoBehaviour
    {
        [SerializeField] private Transform raycastEmitter;
        [SerializeField][Range(3, 100)] private float moveSpeed;
        private Rigidbody2D _rigidbody2D;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            var hit = Physics2D.Raycast(raycastEmitter.position, transform.up * 999);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<PuffStateHandler>())
            {
                // i saw the player!!!!!!! get its shit
                print("SAW YOU");
                _rigidbody2D.AddForce(transform.up * moveSpeed);
            }
        }
    }
}
