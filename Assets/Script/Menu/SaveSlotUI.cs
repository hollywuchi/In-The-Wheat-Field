using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;
    private DataSlot currentData;

    // 获取当前子物体的序号？
    private int Index => transform.GetSiblingIndex();

    void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadSceneData);
    }

    void OnEnable()
    {
        SetupSlotUI();
    }
    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];

        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            dataTime.text = "时间似乎停滞了";
            dataScene.text = "这里漆黑一片";
        }
    }

    private void LoadSceneData()
    {
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(Index);
        }
        else
        {
            EventHandler.CallStartNewGameEvent(Index);
        }
    }
}
