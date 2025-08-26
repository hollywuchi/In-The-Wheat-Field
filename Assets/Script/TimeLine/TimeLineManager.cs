using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : Singleton<TimeLineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;
    private bool isPause;

    private bool isDown;
    public bool IsDown { set => isDown = value; }
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    void OnEnable()
    {
        // currentDirector.played += TimeLinePlayed;
        // currentDirector.stopped += TimeStopped;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    void OnDisable()
    {

    }

    void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space))
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }
    public void PauseTimeLine(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);

        isPause = true;
    }

    // private void TimeLinePlayed(PlayableDirector director)
    // {
    //     if (director != null)
    //         EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    // }

    // private void TimeStopped(PlayableDirector director)
    // {
    //     if (director != null)
    //         EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    // }

    private void OnAfterSceneLoadEvent()
    {
        currentDirector = FindObjectOfType<PlayableDirector>();
        if(currentDirector != null)
            currentDirector.Play();
    }
}
