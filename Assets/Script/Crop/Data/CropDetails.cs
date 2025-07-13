using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int seedItemID;
    [Header("不同阶段所需要的天数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var amou in growthDays)
            {
                amount += amou;
            }
            return amount;
        }
    }

    [Header("不同生长阶段的Prefabs")]
    public GameObject[] growthPrefabs;
    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;
    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("每种工具使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品ID")] // 葡萄收割后会变成葡萄藤
    public int transferItemID;


    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("再次生长时间")]
    public int daysToRegrow;
    public int RegrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

    /// <summary>
    /// 检查工具是否可用，也就是菜篮子不能砍树
    /// </summary>
    /// <param name="toolID">当前的工具ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获取当前工具的使用次数
    /// </summary>
    /// <param name="toolID">当前工具的ID</param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }

}
