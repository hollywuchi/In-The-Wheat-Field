using UnityEngine;

[System.Serializable] // 为了让unity识别，毕竟这只是一个类，没有继承任何东西

public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon; //这个物品在背包里是什么样的
    public Sprite itemOnWorldSprite;//这个物品放在地图上是什么样的（如果可以）
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0,1)]
    public float sellPercentage;
}

[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public struct AnimatorTypes
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}