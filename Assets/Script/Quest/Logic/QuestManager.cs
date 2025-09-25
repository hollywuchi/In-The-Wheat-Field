using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : Singleton<QuestManager>
{
    public QuestData_SO questDataBase;

    void OnEnable()
    {
        EventHandler.AcceptQuest += OnAcceptQuest;
    }

    void OnDisable()
    {
        EventHandler.AcceptQuest -= OnAcceptQuest;
    }
    
    /// <summary>
    /// 承接任务应该做的
    /// </summary>
    /// <param name="details"></param>
    private void OnAcceptQuest(QuestDetails currentDetails)
    {
        var targetQuest = questDataBase.GetQuestData(currentDetails.npcName);
        if(targetQuest != null)
        {
            // 刷新数据库中任务的状态
            targetQuest.questStates = currentDetails.questStates;
        }
    }
}
