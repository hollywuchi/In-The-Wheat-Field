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
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // 直接呼叫开始
        EventHandler.CallDialogueEvent(piece);
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
            EventHandler.CallDialogueEvent(null);
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
        EventHandler.CallDialogueEvent(null);
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
