using UnityEngine;

namespace Other
{
    public class Parallax : MonoBehaviour
    {
        private float length, startpos;
        [SerializeField] private GameObject cam;
        [SerializeField] private float parallaxEffect;

        private void Start()
        {
            startpos = transform.position.x;
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void FixedUpdate()
        {
            var currentCamPos = cam.transform.position;
            var currentPosition = transform.position;
            
            var temp = (currentCamPos.x * (1 - parallaxEffect));
            var dist = (currentCamPos.x * parallaxEffect);

            transform.position = new Vector3(startpos + dist, currentPosition.y, currentPosition.z);

            if (temp > startpos + length)
                startpos += length;
            else if (temp < startpos - length)
                startpos -= length;
        }
    }
}