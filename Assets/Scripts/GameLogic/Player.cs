using ExtensionsFunctions;
using UnityEngine;

namespace GameLogic
{

    [System.Serializable]
    struct PhysicsConfig
    {
        [SerializeField][Range(0.01f, 100)] public float targetMoveSpeed;
        [SerializeField][Range(0.01f, 100)] public float moveAcceleration;
        [SerializeField][Range(0.01f, 100)] public float stoppingAcceleration;
    }

    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private Timer _puffingTimer;

        [SerializeField] PhysicsConfig deflatedPhysicsConfig;
        [SerializeField] PhysicsConfig puffedPhysicsConfig;
        [SerializeField] [Range(3, 7)] private float puffTimeout;
        PuffStateHandler _puffStateHandler;
        Vector2 currentVelocity;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _puffingTimer = GetComponent<Timer>();
            _puffStateHandler = GetComponent<PuffStateHandler>();
        }
        
        private void Update()
        {
            var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            
            float moveAcceleration = _puffStateHandler.IsPuffed ? puffedPhysicsConfig.moveAcceleration : deflatedPhysicsConfig.moveAcceleration;
            float stoppingAcceleration = _puffStateHandler.IsPuffed ? puffedPhysicsConfig.stoppingAcceleration : deflatedPhysicsConfig.stoppingAcceleration;
            float targetMoveSpeed = _puffStateHandler.IsPuffed ? puffedPhysicsConfig.targetMoveSpeed : deflatedPhysicsConfig.targetMoveSpeed;

            currentVelocity = new Vector2(
               Mathf.MoveTowards(currentVelocity.x, direction.x * targetMoveSpeed, direction.x == 0 ? stoppingAcceleration : moveAcceleration),
                 Mathf.MoveTowards(currentVelocity.y, direction.y * targetMoveSpeed, direction.y == 0 ? stoppingAcceleration : moveAcceleration)
                );

            _rigidbody2D.AccelerateTo2D(currentVelocity);   // it freaked out!!! :(
            
            //_rigidbody2D.AddForce(direction * speed * _rigidbody2D.mass);
            // print("Resulting accel: " + acceleration);

            if (Input.GetMouseButton(0))
            {
                Puff();
                _puffingTimer.StartTimer(3);
                _puffingTimer.Resume();
            }
        }

        public void Puff()
        {
            print("OW FUCK PANIC");
            transform.localScale = new Vector2(3, 3);
            _puffStateHandler.SetState(PuffStateHandler.State.Puffed);
        }

        public void Deflate()
        {
            print("calm once again");
            transform.localScale = new Vector2(1, 1);
            _puffStateHandler.SetState(PuffStateHandler.State.Deflated);
        }
    }    
}