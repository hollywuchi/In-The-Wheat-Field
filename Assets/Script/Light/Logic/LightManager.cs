using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currnetSeason;
    private float timeDifference = Settings.lightChangeDuration;

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.LightShiftChangeEvnet += OnLightShiftChangeEvnet;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.LightShiftChangeEvnet -= OnLightShiftChangeEvnet;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        currentLightShift = LightShift.Morning;
    }

    private void OnAfterSceneLoadEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();

        foreach (LightControl light in sceneLights)
        {
            light.ChangeLightShift(currnetSeason, currentLightShift, timeDifference);
        }
    }

    private void OnLightShiftChangeEvnet(Season season, LightShift shift, float timeDifference)
    {
        currnetSeason = season;
        this.timeDifference = timeDifference;
        if (currentLightShift != shift)
        {
            currentLightShift = shift;

            foreach (LightControl light in sceneLights)
            {
                light.ChangeLightShift(currnetSeason, currentLightShift, timeDifference);
            }
        }

    }
}
