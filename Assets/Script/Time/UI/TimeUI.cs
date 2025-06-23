using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    public Sprite[] seasonSprite;

    public List<GameObject> clockBlocks = new List<GameObject>();
    void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        EventHandler.GameDateEvent += OnGameDateEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }

    void OnDisable()
    {
        EventHandler.GameDateEvent -= OnGameDateEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        // ToString("00") 正则化写法，可以显示01,02这种时间
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");

    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }

    private void SwitchHourImage(int hour)
    {
        int Index = hour / 4;

        if (Index == 0)
        {
            foreach (var item in clockBlocks)
            {
                item.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < clockBlocks.Count; i++)
            {
                if (i < Index + 1)
                {
                    clockBlocks[i].SetActive(true);
                }
                else
                {
                    clockBlocks[i].SetActive(false);
                }
            }
        }
    }

    private void DayNightImageRotate(int hour)
    {
        // 因为图像并不是正确的所以要调整度数，-90
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1, RotateMode.Fast);
    }
}
