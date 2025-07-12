using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler 
{
    public static event Action<InventoryLocation,List<InventoryItem>> UpdateInventoryUI; 
    public static void CallUpdateInventoryUI(InventoryLocation location,List<InventoryItem> item)
    {
        UpdateInventoryUI?.Invoke(location,item);
    }

    public static event Action<int,Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID,pos);
    }

    public static event Action<int,Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID,Vector3 pos,ItemType itemType)
    {
        DropItemEvent?.Invoke(ID,pos,itemType);
    }

    public static event Action<ItemDetails,bool> ItemSelectEvent;
    public static void CallItemSelectEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectEvent?.Invoke(itemDetails,isSelected);
    } 

    public static event Action<int,int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int second,int minute)
    {
        GameMinuteEvent?.Invoke(second,minute);
    }

    public static event Action<int,Season> GameDayEvent;
    public static void CallGameDayEvent(int day,Season season)
    {
        GameDayEvent?.Invoke(day,season);
    }
    
    public static event Action<int,int,int,int,Season> GameDateEvent;
    public static void CallGameDateEvent(int hour,int day,int month,int year,Season season)
    {
        GameDateEvent?.Invoke(hour,day,month,year,season);
    }

    public static event Action<string,Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 position)
    {
        TransitionEvent?.Invoke(sceneName,position);
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 position)
    {
        MoveToPosition?.Invoke(position);
    }

    public static event Action<Vector3,ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos,itemDetails);
    }

    public static event Action<Vector3,ItemDetails> ExcuteActionAfterAnimation;
    public static void CallExcuteActionAfterAnimation(Vector3 pos,ItemDetails itemDetails)
    {
        ExcuteActionAfterAnimation?.Invoke(pos,itemDetails);
    }

    public static event Action<int,TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID,TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(ID,tileDetails);
    }
}
