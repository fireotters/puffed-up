using System.Collections.Generic;
using GameLogic.Entities.Enemies;
using Signals;
using UnityEngine;

namespace GameLogic.Camera
{
    public class PlayerDetector : MonoBehaviour
    {
        private PolygonCollider2D _polygonCollider2D;
        [SerializeField] private ContactFilter2D enemyContactFilter;
        [SerializeField] private PlayerDetectorManager _playerDetectorManager;

        public PolygonCollider2D PolygonCollider2D => _polygonCollider2D;

        public void SetManagerTo(PlayerDetectorManager manager) => _playerDetectorManager = manager;

        private void Start()
        {
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() != null)
            {
                _playerDetectorManager.PlayerEntered(this);
                print($"player entered {other.gameObject.name}");
                var bigFishesInCollider = new List<Collider2D>();
                print("start on big fish search");
                _polygonCollider2D.OverlapCollider(enemyContactFilter, bigFishesInCollider);
                
                bigFishesInCollider.ForEach((bigFish) =>
                {
                    if (bigFish.TryGetComponent(out BigFishEnemy bigFishEnemy))
                    {
                        print("enabling big fish");
                        bigFishEnemy.SetBehaviour(true);   
                    }
                });
            }

            
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() != null)
                _playerDetectorManager.PlayerExited(this);
        }
    }
}