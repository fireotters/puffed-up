using UnityEngine;

namespace GameLogic.Entities.Enemies
{
    public class BigFishEnemy : MonoBehaviour
    {
        [SerializeField][Range(0, 100)] private float moveSpeed;
        [SerializeField] private PuffStateSo puffStateSo;
        private Rigidbody2D _rigidbody2D;
        private bool _chasing, _evading;
        private Vector2 _target = Vector2.zero;

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
            }
            else
            {
                // standby behaviour
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                var puffState = puffStateSo.State;
                if (puffState == PuffStateHandler.State.Puffed)
                {
                    _evading = true;
                }
                else
                {
                    _chasing = true;
                    _target = player.gameObject.transform.position;
                    print($"Player is at {_target}");
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                var puffState = puffStateSo.State;
                if (puffState == PuffStateHandler.State.Deflated)
                {
                    _target = player.gameObject.transform.position;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            print("Stopping chase");
            _evading = false;
            _chasing = false;
        }
    }
}