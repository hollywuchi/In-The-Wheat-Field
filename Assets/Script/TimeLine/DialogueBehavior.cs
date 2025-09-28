using System.Security;
using Farm.Dialogue;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehavior : PlayableBehaviour
{
    private PlayableDirector director;

    public DialoguePiece piece;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // 直接呼叫开始
        EventHandler.CallShowDialogueEvent(piece);
        if (Application.isPlaying)
        {
            if (piece.hasToPause)
            {
                // 执行暂停
                TimeLineManager.Instance.PauseTimeLine(director);
            }
        }
        else
        {
            EventHandler.CallShowDialogueEvent(null);
        }
    }

    // 在timeline播放期间每帧执行
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
            TimeLineManager.Instance.IsDown = piece.isDown;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(null);
        // 继承于PlayableBehaviour的方法貌似不遵循Unity的生命周期，因此会在没有编译的情况下调用，从而产生没有实例化的错误
        // TimeLineManager.Instance.isCompleted = true;
    }

    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
    }

    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}
