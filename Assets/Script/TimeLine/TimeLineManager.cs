using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;
using TMPro;

public class TimeLineManager : Singleton<TimeLineManager>
{
    public PlayableDirector startDirector;
    // public Image skipImg;
    // private Text skipText;
    private PlayableDirector currentDirector;
    private bool isPause;

    private bool isDown;
    public bool IsDown { set => isDown = value; }
    private bool isCompleted;
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
        // skipText = transform.GetComponentInChildren<Text>();
    }

    void OnEnable()
    {
        // currentDirector.played += TimeLinePlayed;
        // currentDirector.stopped += TimeStopped;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        // 这是timeline的一个委托可以方便的获得当前timeline播放的状态
        currentDirector.stopped += OnStop;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        currentDirector.stopped -= OnStop;
    }

    private void OnStop(PlayableDirector director) => isCompleted = true;

    void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space) && isDown)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }

        // SkipImageFade();

    }
    public void PauseTimeLine(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);

        isPause = true;
    }

    private void OnAfterSceneLoadEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector != null && !isCompleted)
            currentDirector.Play();
    }

    // 跳过按钮显示
    // 长按按钮，直到完全显示，直接跳过
    //      
    // 中间松手,会逐渐隐藏到消失
    // private void SkipImageFade()
    // {
    //     if (Input.GetKey(KeyCode.Space) && !isCompleted)
    //     {
    //         // 长按空格键跳过剧情
    //         float time = Time.deltaTime;
    //         time += Time.deltaTime;
    //         if (time <= time + 3)   // 如果时间没有到3s
    //         {
    //             if (skipImg.color.a != 1)   // 且图标没有完全显示，那就持续增加a的值直到完全显示
    //             {
    //                 Color imgColor = skipImg.color;
    //                 imgColor.a += 1 / (3 * Time.deltaTime);
    //                 skipImg.color = imgColor;
    //             }
    //         }
    //         else
    //         {
                
    //             time = 0;
    //         }
    //     }
    //     else if (skipImg.color.a > 0)  // 如果没到那么就持续递减直到a变成0
    //     {
    //         Color imgColor = skipImg.color;
    //         imgColor.a -= 1 / (3 * Time.deltaTime);
    //         skipImg.color = imgColor;
    //     }


    // }
}
