using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDetailLsit_SO", menuName = "Inventory/ItemDataList")]
public class ItemDetailList_SO : ScriptableObject
{
    public List<ItemDetails> itemDetailsList;
}
