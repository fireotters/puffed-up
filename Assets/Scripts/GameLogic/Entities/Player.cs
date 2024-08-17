using System;
using ExtensionsFunctions;
using System.Threading;
using UnityEngine;

namespace GameLogic
{
    [System.Serializable]
    struct PhysicsConfig
    {
        [SerializeField] [Range(0.01f, 100)] public float targetMoveSpeed;
        [SerializeField] [Range(0.01f, 100)] public float moveAcceleration;
        [SerializeField] [Range(0.01f, 100)] public float stoppingAcceleration;
    }

    public class Player : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] PhysicsConfig deflatedPhysicsConfig;
        [SerializeField] PhysicsConfig puffedPhysicsConfig;
        private Rigidbody2D _rigidbody2D;
        Vector2 currentVelocity;
        private bool _slowedDown;

        [Header("Abilities")]
        [SerializeField] [Range(3, 7)] private float puffTimeout;
        CancellationTokenSource cancellationToken;
        private Timer _puffingTimer;
        PuffStateHandler _puffStateHandler;
        HealthHandler _healthHandler;

        private void OnDestroy()
        {
            GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _puffingTimer = GetComponent<Timer>();
            _puffStateHandler = GetComponent<PuffStateHandler>();
            _healthHandler = GetComponent<HealthHandler>();
        }
        private void Start()
        {

        }

        private void Update()
        {
            var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            float moveAcceleration = _puffStateHandler.IsPuffed
                ? puffedPhysicsConfig.moveAcceleration
                : deflatedPhysicsConfig.moveAcceleration;
            float stoppingAcceleration = _puffStateHandler.IsPuffed
                ? puffedPhysicsConfig.stoppingAcceleration
                : deflatedPhysicsConfig.stoppingAcceleration;
            float targetMoveSpeed = _puffStateHandler.IsPuffed
                ? puffedPhysicsConfig.targetMoveSpeed
                : deflatedPhysicsConfig.targetMoveSpeed;

            currentVelocity = new Vector2(
                Mathf.MoveTowards(currentVelocity.x, direction.x * targetMoveSpeed,
                    direction.x == 0 ? stoppingAcceleration : moveAcceleration),
                Mathf.MoveTowards(currentVelocity.y, direction.y * targetMoveSpeed,
                    direction.y == 0 ? stoppingAcceleration : moveAcceleration)
            );

            _rigidbody2D.AccelerateTo2D(_slowedDown ? currentVelocity / 2 : currentVelocity);

            if (Input.GetKeyDown(KeyCode.F))
            {
                Puff();
                _puffingTimer.StartTimer(3);
                _puffingTimer.Resume();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag.Equals("Seaweed"))
            {
                _slowedDown = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag.Equals("Seaweed"))
            {
                _slowedDown = false;
            }
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (other.collider.tag)
            {
                case "HurtMinor":
                    _healthHandler.Damage(1);
                    // Insert animator bool and sound
                    break;
                case "HurtMajor":
                    _healthHandler.Damage(3);
                    // Insert animator bool and sound
                    break;
            }
        }

        public void Puff()
        {
            print("OW FUCK PANIC");
            GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
            this.LerpScale(Vector2.one * 3, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);

            _puffStateHandler.SetState(PuffStateHandler.State.Puffed);
        }

        public void Deflate()
        {
            print("calm once again");
            GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
            this.LerpScale(Vector2.one, 0.23f, AnimationCurve.EaseInOut(0, 0, 1, 1), cancellationToken.Token);
            _puffStateHandler.SetState(PuffStateHandler.State.Deflated);
        }
    }
}