using System.Net.Http.Headers;
using Farm.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : Singleton<TimeLineManager>, ISaveable
{
    public PlayableDirector startDirector;
    public CanvasGroup SkipGroup;
    private PlayableDirector currentDirector;
    private bool isPause;

    private bool isDown;
    public bool IsDown { set => isDown = value; }

    public string GUID => GetComponent<DataGUID>().guid;

    private bool isCompleted;

    private float skipTime;
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
        // skipText = transform.GetComponentInChildren<Text>();
    }

    void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
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

        SkipImageFade();

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
    private void SkipImageFade()
    {
        if (Input.anyKey && !isCompleted)
        {
            // 长按空格键跳过剧情
            skipTime += Time.deltaTime;
            if (skipTime <= Settings.SkipTime)   // 如果时间没有到2s
            {
                if (SkipGroup.alpha != 1)   // 且图标没有完全显示，那就持续增加a的值直到完全显示
                {
                    // 这里并不需要进行计算，而是直接进行分数表示线性关系即可
                    SkipGroup.alpha = skipTime / Settings.SkipTime;
                }
            }
            else if (Input.GetKey(KeyCode.Space))   // 如果时间到达了3秒，那么就跳过剧情
            {
                currentDirector.time = 19.9667f;
                // print("跳过剧情");
                skipTime = 0;
                isCompleted = true;
            }
        }
        else if (SkipGroup.alpha > 0)  // 中间松手
        {
            skipTime -= 2 * Time.deltaTime;
            SkipGroup.alpha = skipTime / Settings.SkipTime;
        }


    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.isCompleted = isCompleted;
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.isCompleted = saveData.isCompleted;
    }
}
