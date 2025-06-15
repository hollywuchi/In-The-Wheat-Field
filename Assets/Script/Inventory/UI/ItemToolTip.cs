using System.Collections;
using System.Collections.Generic;
using Farm.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toolTipName;
    [SerializeField] private TextMeshProUGUI toolTipType;
    [SerializeField] private TextMeshProUGUI toolTipIntroduce;
    [SerializeField] private Text Value;
    public GameObject toolTipCoin;

    public void SetUpToolTip(ItemDetails itemDetails, SoltType soltType)
    {
        toolTipName.text = itemDetails.itemName;
        toolTipType.text = GetNameWithType(itemDetails.itemType);
        toolTipIntroduce.text = itemDetails.itemDescription;

        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commondity || itemDetails.itemType == ItemType.Funiture)
        {
            toolTipCoin.SetActive(true);
            var price = itemDetails.itemPrice;
            if (soltType == SoltType.Bag)
            {
                price = (int)(price * itemDetails.sellPercentage);
            }
            Value.text = price.ToString();
        }
        else
        {
            toolTipCoin.SetActive(false);
        }
        // 立刻强制渲染
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private string GetNameWithType(ItemType type)
    {
        return type switch
        {
            ItemType.Seed => "种子",
            ItemType.Commondity => "商品",
            ItemType.Funiture => "家具",
            ItemType.BreakTool => "工具",
            ItemType.HoeTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            ItemType.CollectTool => "工具",
            _ => "无"
        };
    }

}
