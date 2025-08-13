using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDetailLsit_SO", menuName = "Inventory/ItemDataList")]
public class ItemDetailList_SO : ScriptableObject
{
    // [ 警告 ]：不要修改itemDetailsList的名称！！！否则将会重置所有的Item！！！
    public List<ItemDetails> itemDetailsList;

    public ItemDetails GetItemDetails(int ID)
    {
        return itemDetailsList.Find(i => i.itemID == ID);
    }
}
