using UnityEngine;

namespace GameLogic
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private AreaEffector2D areaEffector2D;
        private SpriteRenderer _box;
        [SerializeField] private Sprite openSprite, closedSprite;
        
        private void Start()
        {
            _box = GetComponent<SpriteRenderer>();
        }

        public void EnableChest()
        {
            _box.sprite = openSprite;
            areaEffector2D.enabled = true;
        }


        public void DisableChest()
        {
            _box.sprite = closedSprite;
            areaEffector2D.enabled = false;
        }
    }    
}