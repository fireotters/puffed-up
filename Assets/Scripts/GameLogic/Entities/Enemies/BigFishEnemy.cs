using System;
using System.Threading;
using ExtensionsFunctions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic.Entities.Enemies
{
    public class BigFishEnemy : MonoBehaviour
    {
        [SerializeField][Range(0, 100)] private float moveSpeed;
        [SerializeField] private PuffStateSo puffStateSo;
        private Rigidbody2D _rigidbody2D;
        private bool _chasing, _evading, _normalBehaviourRunning;
        private Vector2 _target = Vector2.zero;
        private CancellationTokenSource _cancellationToken = new();

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            if (_chasing)
            {
                //chasing behaviour
                var direction = _target - (Vector2) transform.position;
                _rigidbody2D.AddForce(direction.normalized * moveSpeed);
            }
            else if (_evading)
            {
                // evading behaviour
                var direction = _target + (Vector2) transform.position;
                _rigidbody2D.AddForce(direction.normalized * moveSpeed);
            }
            else
            {
                // standby behaviour
                if (_normalBehaviourRunning) return;

                var direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                var duration = Random.Range(0f, 4f);
                print($"Imma go {direction} for {Random.Range(0f, 4f)}s!");
                _normalBehaviourRunning = true;
                this.ExecuteOverDuration(duration, _cancellationToken.Token, time =>
                {
                    _rigidbody2D.AddForce(direction * moveSpeed);
                    if (time == 1f) _normalBehaviourRunning = false;
                }).Forget();
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
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            FindPlayerAndSetEnemyState(other);
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
        }

        private void OnDestroy()
        {
            GenericExtensions.CancelAndGenerateNew(ref _cancellationToken);
        }
    }
}