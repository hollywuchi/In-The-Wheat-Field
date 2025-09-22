using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;
    public GameObject menuPrefab;

    public Button settingsButton;
    public GameObject pausePanel;
    public Slider volumeSlide;

    void Awake()
    {
        settingsButton.onClick.AddListener(TogglePausePanel);
        volumeSlide.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }
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
}
