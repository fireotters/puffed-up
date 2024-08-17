using UnityEngine;

namespace GameLogic
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private AreaEffector2D areaEffector2D;
        private SpriteRenderer _box;
        
        private void Start()
        {
            _box = GetComponent<SpriteRenderer>();
        }

        public void EnableChest()
        {
            _box.color = Color.green;
            areaEffector2D.enabled = true;
        }


        public void DisableChest()
        {
            _box.color = Color.red;
            areaEffector2D.enabled = false;
        }
        
        // private void Update()
        // {
        //
        // }
    }    
}