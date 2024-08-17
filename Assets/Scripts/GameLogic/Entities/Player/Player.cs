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

    public class Player : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] PhysicsConfig deflatedPhysicsConfig;
        [SerializeField] PhysicsConfig puffedPhysicsConfig;
        private Rigidbody2D _rigidbody2D;
        Vector2 currentVelocity;
        private int seaweedAffectingPlayer;

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