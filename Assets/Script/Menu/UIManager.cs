using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("菜单组件")]
    private GameObject menuCanvas;
    public GameObject menuPrefab;
    [Header("任务组件")]
    public GameObject questPrefab;
    public GameObject questContent;
    public GameObject questInfoUI;
    public GameObject questRewardUI;
    public GameObject questUI;

    public Button settingsButton;
    public GameObject pausePanel;
    public Slider volumeSlide;

    // 单例模式的awake出现问题
    
    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    void Start()
    {
        settingsButton.onClick.AddListener(TogglePausePanel);
        settingsButton.onClick.AddListener(GetQuestToUI);
        volumeSlide.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);

        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    private void OnAfterSceneLoadEvent()
    {
        if (menuCanvas.transform.childCount > 0)
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
    }

    private void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;

        // 貌似现在已经弃用
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            System.GC.Collect();    // 实现强制垃圾回收
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void GetQuestToUI()
    {
        // 在重新生成UI之前删除所有的原UI
        foreach(Transform obj in questContent.transform)
        {
            Destroy(obj.gameObject);
        }
        // 现在的UI数量正确，但是信息并不正确
        foreach (var quest in QuestManager.Instance.questDataBase.questDataList)
        {
            if (quest.questStates == QuestStates.Accept)
            {
                var uiEle = Instantiate(questPrefab, questContent.transform);
                uiEle.GetComponent<QuestUI>().questDetails = quest;
                uiEle.transform.GetChild(0).GetComponent<Text>().text = quest.questName;
                // EventHandler.CallShowQuestOnUI(quest);
            }
        }
    }

    public void WakeUpUI()
    {
        questUI.SetActive(false);
        questInfoUI.SetActive(true);
    }


    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }

    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        EventHandler.CallEndGameEvent();
        yield return new WaitForSeconds(0.5f);
        // Worning:如果之后出现游戏结束方面的BUG 先来这地方把0.5改成1
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    public void DestoryAllSlot()
    {
        foreach (var reward in questRewardUI.GetComponentsInChildren<SlotUI>())
        {
            Destroy(reward.gameObject);
        }
    }
}
