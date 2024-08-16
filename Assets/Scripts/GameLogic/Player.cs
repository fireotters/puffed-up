using ExtensionsFunctions;
using UnityEngine;

namespace GameLogic
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private Timer _puffingTimer;
        [SerializeField][Range(0.01f, 100)] private float targetMoveSpeed;
        [SerializeField][Range(0.01f, 100)] private float moveAcceleration;
        [SerializeField][Range(0.01f, 100)] private float stoppingAcceleration;
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
            if (direction == Vector2.zero)
                currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, stoppingAcceleration);
            else
                currentVelocity = Vector2.MoveTowards(currentVelocity, direction * targetMoveSpeed, moveAcceleration);

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