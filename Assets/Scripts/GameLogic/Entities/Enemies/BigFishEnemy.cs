using System;
using System.Threading;
using ExtensionsFunctions;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic.Entities.Enemies
{
    public class BigFishEnemy : MonoBehaviour
    {
        [SerializeField][Range(0, 100)] private float moveSpeed;
        [SerializeField] private PuffStateSo puffStateSo;
        private Rigidbody2D _rigidbody2D;
        private bool _chasing, _evading, _normalBehaviourRunning, _standby;
        private Vector2 _target = Vector2.zero;
        private CancellationTokenSource _cancellationToken = new();
        private Animator _animator;

        // Sound
        [SerializeField] private StudioEventEmitter sndEnemyTriggered, sndEnemyBounce;
        private float checkTimeBounceSfx, delayBetweenBounceSfx = 1f;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            checkTimeBounceSfx = Time.time + delayBetweenBounceSfx;
        }

        private void Update()
        {
            if (!_standby) return;

            Vector2 direction = new Vector2();
            if (_chasing)
            {
                //chasing behaviour
                direction = _target - (Vector2)transform.position;
                _rigidbody2D.AddForce(direction.normalized * moveSpeed);
                _animator.SetFloat("speed", _rigidbody2D.velocity.magnitude / 7);
                int swimType = (int)Math.Round(Random.Range(1f, 2f));
                _animator.Play("Swim" + swimType);
            }
            else if (_evading)
            {
                // evading behaviour
                direction = _target + (Vector2)transform.position;
                _rigidbody2D.AddForce(direction.normalized * moveSpeed);
                _animator.SetFloat("speed", _rigidbody2D.velocity.magnitude / 7);
                int swimType = (int)Math.Round(Random.Range(1f, 2f));
                _animator.Play("Scare" + swimType);
            }
            else
            {
                // standby behaviour
                if (_normalBehaviourRunning) return;

                var duration = Random.Range(0f, 4f);
                if (Random.Range(0, 2) == 1)
                {
                    direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                    //print($"Imma go {direction} for {Random.Range(0f, 4f)}s!");
                    _normalBehaviourRunning = true;
                    this.ExecuteOverDuration(duration, _cancellationToken.Token, time =>
                    {
                        _rigidbody2D.AddForce(direction * moveSpeed);
                        if (time == 1f) _normalBehaviourRunning = false;
                    }).Forget();
                }
                else
                {
                    //print("I aint movin");
                    _animator.Play("Idle");
                    _normalBehaviourRunning = true;
                    this.ExecuteOverDuration(duration, _cancellationToken.Token, time =>
                    {
                        // just STOP like STOP IT ALREADY FUCKIN DONT MOVE
                        if (time == 1f) _normalBehaviourRunning = false;
                    }).Forget();
                }
            }
            // Rotate sprite
            if (direction.x > 0f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
            else if (direction.x < -0f)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
            }
        }

        private void FindPlayerAndSetEnemyState(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                _target = player.gameObject.transform.position;
                // print($"Player is at {_target}");
                var puffState = puffStateSo.State;
                _evading = puffState == PuffStateHandler.State.Puffed;
                _chasing = puffState == PuffStateHandler.State.Deflated;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            GenericExtensions.CancelAndGenerateNew(ref _cancellationToken);
            _normalBehaviourRunning = false;
            if (other.gameObject.TryGetComponent(out Player player) == false)
            {
                if (Time.time > checkTimeBounceSfx)
                {
                    checkTimeBounceSfx = Time.time + delayBetweenBounceSfx;
                    sndEnemyBounce.Play();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            FindPlayerAndSetEnemyState(other);
            if (other.gameObject.TryGetComponent(out Player player))
            {
                sndEnemyTriggered.Play();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            FindPlayerAndSetEnemyState(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // print("Stopping chase");
            _evading = false;
            _chasing = false;
            if (other.gameObject.TryGetComponent(out Player player))
            {
                sndEnemyTriggered.Play();
            }
        }

        public void SetBehaviour(bool enabled)
        {
            _standby = enabled;
        }
        
        private void OnDestroy()
        {
            GenericExtensions.CancelAndGenerateNew(ref _cancellationToken);
        }
    }
}