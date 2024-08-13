using UnityEngine;

namespace Other
{
    public class SineWaveMover : MonoBehaviour
    {
        [SerializeField] private float oscilationSpeed;
        [SerializeField] private float amplitude;
        private Vector3 initPos;
        private float timer = 0;

        private void Start()
        {
            initPos = transform.localPosition;
        }

        private void Update()
        {
            timer += Time.deltaTime * oscilationSpeed;
            transform.localPosition = amplitude * Mathf.Sin(timer) * Vector3.up + initPos;
        }
    }
}