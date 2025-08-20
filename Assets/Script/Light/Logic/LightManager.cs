using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currnetSeason;
    private float timeDifference;

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.LightShiftChangeEvnet += OnLightShiftChangeEvnet;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.LightShiftChangeEvnet -= OnLightShiftChangeEvnet;
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
