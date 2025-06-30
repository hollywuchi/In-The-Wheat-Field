using UnityEngine;

namespace Farm.Inventory
{
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;
        private SpriteRenderer shadowSprite;

        void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            shadowSprite.sprite = itemSprite.sprite;
            shadowSprite.color = new Color(0, 0, 0, 0.3f);
        }
    }
}
