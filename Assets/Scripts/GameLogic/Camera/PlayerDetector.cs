using Signals;
using UnityEngine;

namespace GameLogic.Camera
{
    public class PlayerDetector : MonoBehaviour
    {
        private PolygonCollider2D _polygonCollider2D;

        private void Start()
        {
            _polygonCollider2D = GetComponent<PolygonCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() != null)
                SignalBus<SignalSwitchCameraBoundary>.Fire(new SignalSwitchCameraBoundary
                    { ColliderToSwitch = _polygonCollider2D });
        }
    }
}