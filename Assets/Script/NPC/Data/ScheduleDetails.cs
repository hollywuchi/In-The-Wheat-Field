using System;
using UnityEngine;

[Serializable]    
public class ScheduleDetails : IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public int priority;  // 优先级，越小越先执行
    public Season season;
    public string targetScene;
    public Vector2Int targetGridPosition;
    public AnimationClip clipAtStop;
    public bool interactable;

    // public ScheduleDetails()
    // {

    // }

    private int Time => (hour * 100) + minute;
    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
                return 1;
            else
                return -1;
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time)
        {
            return -1;
        }

        return 0;
    }
}
