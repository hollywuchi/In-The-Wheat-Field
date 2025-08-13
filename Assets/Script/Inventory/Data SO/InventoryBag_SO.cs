using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBag_SO", menuName = "Inventory/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> BagItemList = new List<InventoryItem>();

    public InventoryItem GetInventoryItem(int iD)
    {
        return BagItemList.Find(i => i.itemID == iD);
    }
}
