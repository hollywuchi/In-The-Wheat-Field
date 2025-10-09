using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Farm.Inventory;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;
    private bool isOpen;

    private Questable questable;

    void Awake()
    {
        questable = GetComponent<Questable>();
    }
    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        if (questable.questDetails.questStates == QuestStates.Complete)
        {
            isOpen = true;
            EventHandler.CallBaseBagOpenEvent(SoltType.Shop, shopData);
            EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        }
    }
    public void CloseShop()
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SoltType.Shop, shopData);
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }


    /// <summary>
    /// 对话结束之后承接任务的事件
    /// </summary>
    public void QuestFunciton()
    {
        // 确保其中的任务不会一口气全部完成
        if (questable.questDetails.questStates == QuestStates.Waitting)
            AcceptQuest();
        else
            CheckQuest();
    }

    private void AcceptQuest()
    {
        if (questable != null)
        {
            questable.questDetails.questStates = QuestStates.Accept;    // 将NPC身上的任务状态转换为承接状态
            EventHandler.CallAcceptQuest(questable.questDetails);   // 当前NPC身上有任务模块才可以承接任务
        }
    }

    /// <summary>
    /// 检测并刷新任务状态
    /// </summary>
    private void CheckQuest()
    {
        if (questable != null && questable.questDetails.questStates == QuestStates.Accept)
        {
            int currentNum = InventoryManager.Instance.Getitem(questable.questDetails.requireItem.itemID).itemAmount;
            // 如果背包中的物品大于所需要的物品
            if (currentNum != 0)
            {
                if (currentNum >= questable.questDetails.requireItem.itemAmount)
                {
                    //  呼叫交付物品的方法,并将当前状态变为完成
                    questable.questDetails.questStates = QuestStates.Complete;
                    if (questable.questDetails.npcName == gameObject.name)
                        EventHandler.CallDeliveryQuestItems(questable.questDetails);
                }
            }
            // 如果不需要物品，那么就直接完成任务就可以
            // 适用于对话类型的任务
            else if (questable.questDetails.requireTime != Vector3Int.zero)
            {
                var questTime = questable.questDetails.requireTime;
                if (TimeSpan.Compare(new TimeSpan(questTime.x, questTime.y, questTime.z), TimeManager.Instance.GameTime) == -1)
                {
                    questable.questDetails.questStates = QuestStates.Complete;
                }
            }
        }
    }
}
