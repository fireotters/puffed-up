using Signals;
using UnityEngine;

namespace GameLogic.Camera
{
    public class PlayerDetector : MonoBehaviour
    {
        private PolygonCollider2D _polygonCollider2D;
        [SerializeField] PlayerDetectorManager _playerDetectorManager;

        public PolygonCollider2D PolygonCollider2D => _polygonCollider2D;

        public void SetManagerTo(PlayerDetectorManager manager) => _playerDetectorManager = manager;

        private void Start()
        {
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() != null)
                _playerDetectorManager.PlayerEntered(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() != null)
                _playerDetectorManager.PlayerExited(this);
        }
    }
}