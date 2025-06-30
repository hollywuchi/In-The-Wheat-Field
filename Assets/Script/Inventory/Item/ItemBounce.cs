using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Farm.Inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;
        private BoxCollider2D coll;

        public float gravity;
        private bool isGround;
        private float distance;
        private Vector2 direction;
        private Vector3 targetPos;

        void Awake()
        {
            spriteTrans = transform.GetChild(0);
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }

        void Update()
        {
            Bounce();
        }

        public void InitBounceItem(Vector3 target, Vector2 dir)
        {
            coll.enabled = false;
            targetPos = target;
            direction = dir;
            distance = Vector3.Distance(target, transform.position);

            spriteTrans.position += Vector3.up * 1.5f;
        }

        private void Bounce()
        {
            isGround = spriteTrans.position.y - transform.position.y < 0.1;

            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
            }

            if (!isGround)
            {
                spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                spriteTrans.position = transform.position;
                var itemSprite = spriteTrans.gameObject.GetComponent<SpriteRenderer>();
                var newoffset = itemSprite.bounds.center.y - transform.position.y;
                coll.offset = new Vector2(0, newoffset);
                coll.enabled = true;
            }
        }
    }
}
