using UnityEngine;

namespace GameLogic
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private Timer _puffingTimer;
        [SerializeField][Range(0.25f, 1)] private float speed;
        [SerializeField] [Range(3, 7)] private float puffTimeout;
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _puffingTimer = GetComponent<Timer>();
        }
        
        private void Update()
        {
            var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // var acceleration = _rigidbody2D.AccelerateTo2D(direction * speed);   // it freaked out!!! :(
            _rigidbody2D.AddForce(direction * speed);
            // print("Resulting accel: " + acceleration);

            if (Input.GetMouseButton(0))
            {
                PuffOut();
                _puffingTimer.StartTimer(3);
                _puffingTimer.Resume();
            }
        }

        public void PuffOut()
        {
            print("OW FUCK PANIC");
            transform.localScale = new Vector2(3, 3);
        }

        public void PuffIn()
        {
            print("calm once again");
            transform.localScale = new Vector2(1, 1);
        }
    }    
}