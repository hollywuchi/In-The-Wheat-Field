using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace Farm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;
        public ItemDetails itemdetails;
        public SpriteRenderer itemSprte;
        private BoxCollider2D coll => GetComponent<BoxCollider2D>();

        void Start()
        {
            itemSprte = GetComponentInChildren<SpriteRenderer>();
            if (itemID != 0)
                Init(itemID);
        }

        public void Init(int ID)
        {
            itemID = ID;
            itemdetails = InventoryManager.Instance.GetDetails(ID);
            if (itemdetails != null)
                // 如果数据库没有世界图片，那就显示物品本身的图片，防止报空
                itemSprte.sprite = itemdetails.itemOnWorldSprite != null ? itemdetails.itemOnWorldSprite : itemdetails.itemIcon;

            // 自动调整物品的碰撞箱大小
            coll.size = new Vector2(itemSprte.bounds.size.x, itemSprte.bounds.size.y);
            // 修正：itemSprte.bounds.center.y 返回世界坐标的图片中心，直接放到偏移量上肯定不行
            // 所以要减去本来的世界坐标，才可以求出其中的差
            var newoffset = itemSprte.bounds.center.y - transform.position.y;
            coll.offset = new Vector2(0, newoffset);
        }
    }

}
