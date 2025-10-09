using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Farm.Save;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : Singleton<QuestManager>, ISaveable
{
    public QuestData_SO questDataBase;

    public string GUID => GetComponent<DataGUID>().guid;

    void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

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
        if (targetQuest != null)
        {
            // 刷新数据库中任务的状态
            targetQuest.questStates = currentDetails.questStates;
        }
    }

    public QuestDetails RefreshQuestDetails(string npcName)
    {
        return questDataBase.GetQuestData(npcName);
    }

    /// <summary>
    /// 需要保存的
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.questDetails = questDataBase.questDataList;

        return gameSaveData;
    }

    /// <summary>
    /// 需要进行读取的
    /// </summary>
    /// <param name="saveData"></param>
    public void RestoreData(GameSaveData saveData)
    {
        questDataBase.questDataList = saveData.questDetails;
        EventHandler.CallRefreshQuestDetails();
    }
}
