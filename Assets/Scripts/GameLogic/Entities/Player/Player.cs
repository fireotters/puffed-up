using System;
using System.Threading;
using ExtensionsFunctions;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameLogic
{
    [Serializable]
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
        CancellationTokenSource cancellationToken;

        [Header("Abilities")]
        [SerializeField] [Range(3, 7)] private float puffTimeout;
        private Timer _puffingTimer;
        private float lastDeflateTime, waitFromDeflateToNextInflate = 2;
        PuffStateHandler _puffStateHandler;

        [Header("Animation")]
        private Animator _animator;
        private string currentAnimaton = "Idle";

        [Header("Sound")]
        [SerializeField] private StudioEventEmitter sndPlrMoveSmall;
        [SerializeField] private StudioEventEmitter sndPlrMoveBig, sndPlrInflate, sndPlrDeflate, sndPlrBounce, sndPlrDamage, sndPlrDeathNormal, sndPlrDeathExplode;

        private void OnDestroy()
        {
            GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _puffingTimer = GetComponent<Timer>();
            _puffStateHandler = GetComponentInChildren<PuffStateHandler>();
            lastDeflateTime = Time.time;
        }

        private void Update()
        {
            UpdateMovement();
            UpdateAbilities();
        }

        private void UpdateMovement()
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
                stopped = currentVelocity.magnitude < 0.05f;

            if (currentVelocity.magnitude < 0.05f || stopped)
            {
                stopped = true;
                float targetSpeed = _puffStateHandler.IsPuffed ? floatUpSpeed : floatDownSpeed;
                float yDir = _puffStateHandler.IsPuffed ? 1 : -1;
                currentVelocity = new Vector2(currentVelocity.x, Mathf.MoveTowards(currentVelocity.y, yDir * targetSpeed * _rigidbody2D.mass, config.moveAcceleration));
            }

            bool slowedDown = seaweedAffectingPlayer > 0;
            _rigidbody2D.AccelerateTo2D(slowedDown ? currentVelocity / 2 : currentVelocity);
            MovementHandler(direction, slowedDown ? currentVelocity / 2 : currentVelocity);
        }

        private void UpdateAbilities()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Puff();
            }
        }

        public void SeaweedAffect() => seaweedAffectingPlayer++;

        public void RemoveSeaweedAffect() => seaweedAffectingPlayer--;

        public void MovementHandler(Vector2 direction, Vector2 trueVelocity)
        {
            PhysicsConfig config = _puffStateHandler.IsPuffed ? puffedPhysicsConfig : deflatedPhysicsConfig;
            var swimName = (_puffStateHandler.IsPuffed ? "Inf_" : "") + "Swim";

            if (direction.x > 0f)
            {
                this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                if (!currentAnimaton.StartsWith(swimName) && !currentAnimaton.Contains("Hurt") && !currentAnimaton.Contains("Death"))
                {
                    int swimType = (int)Math.Round(Random.Range(1f, 2f));
                    ChangeAnimationState("Swim" + swimType);
                }
            }
            else if (direction.x < -0f)
            {
                this.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                if (!currentAnimaton.StartsWith(swimName) && !currentAnimaton.Contains("Hurt") && !currentAnimaton.Contains("Death"))
                {
                    int swimType = (int)Math.Round(Random.Range(1f, 2f));
                    ChangeAnimationState("Swim" + swimType);
                }
            }
            else
            {
                if(!currentAnimaton.Contains("Hurt") && !currentAnimaton.Contains("Death"))
                    ChangeAnimationState("Idle");
            }

            _animator.SetFloat("speed", (trueVelocity / config.targetMoveSpeed).magnitude);
        }

        public void Hurt()
        {
            sndPlrDamage.Play();
            ChangeAnimationState("Hurt");
        }

        public void Die()
        {
            sndPlrDeathNormal.Play();
            ChangeAnimationState("Death");
        }

        public void Puff()
        {
            if (!_puffStateHandler.IsPuffed && lastDeflateTime < Time.time - waitFromDeflateToNextInflate)
            {
                _puffingTimer.StartTimer(3);
                _puffingTimer.Resume();
                sndPlrInflate.Play();
                print("OW FUCK PANIC");
                _puffStateHandler.SetState(PuffStateHandler.State.Puffed);
            }
        }

        public void Deflate()
        {
            lastDeflateTime = Time.time;
            sndPlrDeflate.Play();
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

        // quick animation manager :3c
        void ChangeAnimationState(string newAnimation)
        {
            var trueAnimName = (_puffStateHandler.IsPuffed ? "Inf_" : "") + newAnimation;
            if (currentAnimaton == trueAnimName) return;

            _animator.Play(trueAnimName);
            currentAnimaton = trueAnimName;
        }
    }
}