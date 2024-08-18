using System;
using ExtensionsFunctions;
using UnityEngine;

namespace GameLogic.Entities.Enemies
{
    public class SwordFishEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject swordTip;
        [SerializeField][Range(30, 100)] private float moveSpeed;
        [SerializeField] private LayerMask wallLayer, targetLayerMask;
        private Rigidbody2D _rigidbody2D;
        private bool _charging;
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            var hit = Physics2D.Raycast(swordTip.transform.position, transform.up, Mathf.Infinity, targetLayerMask);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Player>())
            {
                // i saw the player!!!!!!! get its shit
                print("SAW YOU");
                _charging = true;
            }
            
            if (_charging)
                _rigidbody2D.AddForce(transform.up * moveSpeed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.IsInLayerMask(wallLayer))
            {
                print("tip touched wall!");    
            }
        }
    }
}
