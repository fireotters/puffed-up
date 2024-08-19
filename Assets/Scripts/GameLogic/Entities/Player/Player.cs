using ExtensionsFunctions;
using FMODUnity;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] float idleSinkSpeedWhenSmall; // Used when deflated and speed close to 0 or ded
        [SerializeField] float idleFloatSpeedWhenBig; // Used when puffed and speed close to 0 or ded
        private Rigidbody2D _rigidbody2D;
        Vector2 currentVelocity;
        private int seaweedAffectingPlayer;
        bool stopped = false;
        CancellationTokenSource cancellationToken;

        [Header("Abilities")]
        [SerializeField] [Range(0, 7)] private float puffTimeout;
        private Timer _puffingTimer;
        PuffStateHandler _puffStateHandler;
        private bool isBoosting = false;
        [SerializeField] private float boostDuration, boostForce;
        [SerializeField] private GameObject puffPushEffector;

        [Header("Animation")]
        private Animator _animator;
        private string currentAnimaton = "Idle";
        private float hurtAnimDuration = 0.3f;
        [SerializeField] private ParticleSystem _particlesBoostBubbles;

        [Header("Sound")]
        [SerializeField] private StudioEventEmitter sndPlrMoveSmall;
        [SerializeField] private StudioEventEmitter sndPlrMoveBig, sndPlrInflate, sndPlrDeflate, sndPlrBounce, sndPlrDamage, sndPlrDeathNormal, sndPlrDeathExplode;

        [Header("Vitals")]
        private HealthHandler _healthHandler;

        private void OnDestroy()
        {
            GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        }

        private void Awake()
        {
            Application.targetFrameRate = 60; // For now... 
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _puffingTimer = GetComponent<Timer>();
            _puffStateHandler = GetComponentInChildren<PuffStateHandler>();
            _healthHandler = GetComponent<HealthHandler>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Level02");
            }
            UpdateMovement();
            UpdateAbilities();
        }

        private void UpdateMovement()
        {
            Vector2 direction = GetMovementDirection();

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
                float targetSpeed = _puffStateHandler.IsPuffed ? idleFloatSpeedWhenBig : idleSinkSpeedWhenSmall;
                float yDir = _puffStateHandler.IsPuffed ? 1 : -1;
                currentVelocity = new Vector2(currentVelocity.x, Mathf.MoveTowards(currentVelocity.y, yDir * targetSpeed * _rigidbody2D.mass, config.moveAcceleration));
            }

            // Final movement calc
            bool slowedDown = seaweedAffectingPlayer > 0;
            MoveAnimator(direction, slowedDown ? currentVelocity / 2 : currentVelocity);
            if (isBoosting) // While boosting, only let code above determine Animator, not movement. TODO ask Rioni/Benchi about this
                return;
            _rigidbody2D.AccelerateTo2D(slowedDown ? currentVelocity / 2 : currentVelocity);

            // Sound
            float movePitch = currentVelocity.magnitude / targetMoveSpeed * 0.8f; // Keep within 0.0f - 0.8f
            if (_puffStateHandler.IsPuffed)
                movePitch /= 2f; // Pitch down when beeg
            sndPlrMoveSmall.SetParameter("Movement_Pitch", movePitch);
            if (!stopped && !sndPlrMoveSmall.IsPlaying()) {
                sndPlrMoveSmall.Play();
            }
            else if (stopped && sndPlrMoveSmall.IsPlaying()) {
                sndPlrMoveSmall.Stop();
            }
            // TODO: Implement Player_Movement_Thicc sfx - I'm unsure why it doesn't work like Player_Movement
        }

        private Vector2 GetMovementDirection()
        {
            if (_healthHandler.IsAlive)
                return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            return Vector2.zero;
        }

        private void UpdateAbilities()
        {
            if (!_healthHandler.IsAlive)
                return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (_puffStateHandler.IsDeflated && _puffStateHandler.CanInflateAgainYet)
                    Puff();
                else if (_puffStateHandler.IsPuffed)
                    Deflate();
            }
        }

        public void SeaweedAffect() => seaweedAffectingPlayer++;

        public void RemoveSeaweedAffect() => seaweedAffectingPlayer--;

        public void MoveAnimator(Vector2 direction, Vector2 trueVelocity)
        {
            PhysicsConfig config = _puffStateHandler.IsPuffed ? puffedPhysicsConfig : deflatedPhysicsConfig;
            var swimName = (_puffStateHandler.IsPuffed ? "Inf_" : "") + "Swim";

            if (direction.x > 0f)
            {
                this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                if (!currentAnimaton.StartsWith(swimName) && !currentAnimaton.Contains("Hurt") && !currentAnimaton.Contains("Death") && !currentAnimaton.Contains("To"))
                {
                    int swimType = (int)Math.Round(Random.Range(1f, 2f));
                    ChangeAnimationState("Swim" + swimType);
                }
            }
            else if (direction.x < -0f)
            {
                this.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                if (!currentAnimaton.StartsWith(swimName) && !currentAnimaton.Contains("Hurt") && !currentAnimaton.Contains("Death") && !currentAnimaton.Contains("To"))
                {
                    int swimType = (int)Math.Round(Random.Range(1f, 2f));
                    ChangeAnimationState("Swim" + swimType);
                }
            }
            else
            {
                if(!currentAnimaton.Contains("Hurt") && !currentAnimaton.Contains("Death") && !currentAnimaton.Contains("To"))
                    ChangeAnimationState("Idle");
            }

            _animator.SetFloat("speed", (trueVelocity / config.targetMoveSpeed).magnitude);
            puffPushEffector.transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.y, 0f));
        }

        public void Hurt()
        {
            sndPlrDamage.SetParameter("Inflated", _puffStateHandler.IsPuffed ? 1 : 0);
            sndPlrDamage.SetParameter("Player_Life", _healthHandler.currentHealth);
            sndPlrDamage.Play();
            ChangeAnimationState("Hurt");
            Invoke(nameof(NoLongerHurt), hurtAnimDuration);
        }

        private void NoLongerHurt()
        {
            ChangeAnimationState("Idle");
        }

        public void Die()
        {
            sndPlrDeathNormal.Play();
            ChangeAnimationState("Death");

            // I removed PuffStateHandler.OnDeath method because of the first SOLID principle.
            // PuffStateHandler mustn't know about lives, dead or any other state other than the puff state.
            _rigidbody2D.gravityScale = 2f;
            // Given this gets called in a single frame it won't cause performance issues
            // It doesn't make sure to have a member variable just for this, getting it on the fly is good in this case
        }

        public void Puff()
        {
            if (_healthHandler.IsAlive)
            {
                ChangeAnimationState("DefToInf");
                sndPlrInflate.Play();
                _puffStateHandler.SetState(PuffStateHandler.State.Puffed);
                StartCoroutine(InflatePush());
            }
        }

        public void Deflate()
        {
            if (_healthHandler.IsAlive)
            {
                sndPlrDeflate.Play();
                _puffStateHandler.SetState(PuffStateHandler.State.Deflated);
                StartCoroutine(Boost());
            }
        }

        private IEnumerator Boost()
        {
            ChangeAnimationState("InfToDef");
            isBoosting = true;
            _particlesBoostBubbles.Play();
            int boostDir = transform.rotation.y == 0 ? 1 : -1; // Boost feesh depending on sprite's facing direction.
            _rigidbody2D.velocity = new Vector2(boostDir * boostForce, 0);
            yield return new WaitForSeconds(0.5f);
            _particlesBoostBubbles.Stop();
            isBoosting = false;
            currentAnimaton = "Idle";

        }

        private IEnumerator InflatePush()
        {
            puffPushEffector.SetActive(true);
            yield return new WaitForSeconds(0.08f);
            puffPushEffector.SetActive(false);
            currentAnimaton = "Inf_Idle";
        }

        public void WasCrushed()
        {
            if (_healthHandler.IsAlive && _puffStateHandler.IsPuffed)
            {
                sndPlrDeflate.Play(); // TODO: Create a sound for 'crush' - like an "Eep!" sfx
                _puffStateHandler.SetState(PuffStateHandler.State.Deflated);
            }
        }

        public void ResetVelocity()
        {
            currentVelocity = Vector2.zero;
        }

        public void AddImpulse(Vector2 impulseForce, bool resetPreviousVelocity = true)
        {
            if (resetPreviousVelocity)
                ResetVelocity();

            _rigidbody2D.AddForce(impulseForce, ForceMode2D.Impulse);
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