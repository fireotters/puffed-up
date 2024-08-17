using System;
using ExtensionsFunctions;
using System.Threading;
using UnityEngine;


/// Añadir turn speed, rebotar en las paredes y que baje/suba (según el estado) cuando la magnitud de la velocidad esté cerca de 0
/// Muerto también flota hacia arriba

namespace GameLogic
{
    [System.Serializable]
    struct PhysicsConfig
    {
        [SerializeField] [Range(0.01f, 100)] public float targetMoveSpeed;
        [SerializeField] [Range(0.01f, 100)] public float moveAcceleration;
        [SerializeField] [Range(0.01f, 100)] public float turnAcceleration;
        [SerializeField] [Range(0.01f, 100)] public float stoppingAcceleration;
    }

    interface ICustomPhysics
    {
        public void AddImpulse(Vector2 impulseForce, bool resetPreviousVelocity = true);
        public void ResetVelocity();
    }

    public class Player : MonoBehaviour, ICustomPhysics
    {
        [Header("Physics")]
        [SerializeField] PhysicsConfig deflatedPhysicsConfig;
        [SerializeField] PhysicsConfig puffedPhysicsConfig;
        [SerializeField] float floatDownSpeed; // Used when deflated and speed close to 0
        [SerializeField] float floatUpSpeed; // Used when puffed and speed close to 0 or ded
        private Rigidbody2D _rigidbody2D;
        Vector2 currentVelocity;
        private int seaweedAffectingPlayer;
        bool stopped = false;

        [Header("Abilities")]
        [SerializeField] [Range(3, 7)] private float puffTimeout;
        private Timer _puffingTimer;
        PuffStateHandler _puffStateHandler;
        HealthHandler _healthHandler;

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

            PhysicsConfig config = _puffStateHandler.IsPuffed ? puffedPhysicsConfig : deflatedPhysicsConfig;

            float moveAcceleration = config.moveAcceleration;
            float stoppingAcceleration = config.stoppingAcceleration;
            float targetMoveSpeed = config.targetMoveSpeed;
            float turnAcceleration = config.turnAcceleration;

            currentVelocity = new Vector2(
                Mathf.MoveTowards(currentVelocity.x, direction.x * targetMoveSpeed,
                    direction.x == 0 ? stoppingAcceleration :
                    Mathf.Sign(direction.x) == Mathf.Sign(currentVelocity.x) ? moveAcceleration : turnAcceleration),
                Mathf.MoveTowards(currentVelocity.y, direction.y * targetMoveSpeed,
                    direction.y == 0 ? stoppingAcceleration :
                    Mathf.Sign(direction.y) == Mathf.Sign(currentVelocity.y) ? moveAcceleration : turnAcceleration)
            );

            if (stopped && direction != Vector2.zero)
                stopped =  currentVelocity.magnitude < 0.05f;

            if (currentVelocity.magnitude < 0.05f || stopped)
            {
                stopped = true;
                float targetSpeed = _puffStateHandler.IsPuffed ? floatUpSpeed : floatDownSpeed;
                float yDir = _puffStateHandler.IsPuffed ? 1 : -1;
                currentVelocity = new Vector2 (currentVelocity.x,  Mathf.MoveTowards(currentVelocity.y, yDir * targetSpeed * _rigidbody2D.mass, config.moveAcceleration));
            }

            bool slowedDown = seaweedAffectingPlayer > 0;
            _rigidbody2D.AccelerateTo2D(slowedDown ? currentVelocity / 2 : currentVelocity);

            if (Input.GetKeyDown(KeyCode.F))
            {
                Puff();
                _puffingTimer.StartTimer(3);
                _puffingTimer.Resume();
            }
        }

        public void SeaweedAffect() => seaweedAffectingPlayer++;

        public void RemoveSeaweedAffect() => seaweedAffectingPlayer--;

        public void Puff()
        {
            print("OW FUCK PANIC");
            _puffStateHandler.SetState(PuffStateHandler.State.Puffed);
        }

        public void Deflate()
        {
            print("calm once again");
            _puffStateHandler.SetState(PuffStateHandler.State.Deflated);
        }

        public void ResetVelocity()
        {
            currentVelocity = Vector2.zero;
        }

        public void AddImpulse(Vector2 impulseForce, bool resetPreviousVelocity = true)
        {
            if (resetPreviousVelocity)
                ResetVelocity();

            this._rigidbody2D.AddForce(impulseForce, ForceMode2D.Impulse);
        }
    }
}