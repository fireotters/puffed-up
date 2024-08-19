using System;
using ExtensionsFunctions;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic.Entities.Enemies
{
    public class SwordFishEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject swordTip;
        [SerializeField][Range(30, 100)] private float moveSpeed;
        [SerializeField] private LayerMask wallLayer, targetLayerMask;
        private Rigidbody2D _rigidbody2D;
        private bool _charging;
        [SerializeField] private StudioEventEmitter sndRazorAttack, sndRazorStick;
        private bool alreadyPlayedAttackSnd = false;
        private Animator _animator;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            var hit = Physics2D.Raycast(swordTip.transform.position, transform.up, Mathf.Infinity, targetLayerMask);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Player>())
            {
                // i saw the player!!!!!!! get its shit
                // print("SAW YOU");
                _charging = true;
                if (!alreadyPlayedAttackSnd)
                {
                    int swimType = (int)Math.Round(Random.Range(1f, 2f));
                    _animator.Play("Swim" + swimType);
                    sndRazorAttack.Play();
                    alreadyPlayedAttackSnd = true;
                    _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
            if (_charging)
            {
                _animator.SetFloat("speed", _rigidbody2D.velocity.magnitude / 7);
                _rigidbody2D.AddForce(transform.up * moveSpeed);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            // If tip touches a wall, play Stick sfx & force enemy to stay still
            if (col.otherCollider.gameObject == swordTip)
            {
                if (col.gameObject.IsInLayerMask(wallLayer))
                {
                    sndRazorStick.Play();
                    _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                    _animator.Play("Idle");
                    // print("tip touched wall!");
                }
            }
        }
    }
}
