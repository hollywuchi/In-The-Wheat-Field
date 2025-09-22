using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData_SO", menuName = "Quest/QuestData_SO")]
public class QuestData_SO : ScriptableObject
{
    public List<QuestDetails> questDataList;

    /// <summary>
    /// 获取当前NPC的任务
    /// </summary>
    /// <param name="npcName"></param>
    /// <returns></returns>
    public QuestDetails GetQuestData(string npcName)
    {
        return questDataList.Find(i => i.npcName == npcName);
    }
}

[System.Serializable]
public class QuestDetails
{
    public string questName;                       // 任务名称
    public string npcName;                         // 是哪个NPC应该携带这个任务
    public QuestStates questStates;                // 任务状态
    [TextArea] public string questIntroduction;    // 任务介绍
    public InventoryItem requireItem;              // 需要的物品

    public List<InventoryItem> rewards;            // 奖励列表，可以根据其中的itemID生成对应的SlotUI
}