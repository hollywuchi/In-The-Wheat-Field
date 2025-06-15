using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Inventory
{
    public class ItemPickedUp : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collision)
        {
            Item item = collision.GetComponent<Item>();
            if(item != null && item.itemdetails.canPickedup)
            {
                InventoryManager.Instance.AddItem(item,true);
            }
        }
    }
}
