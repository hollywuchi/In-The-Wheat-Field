using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Farm.Dialogue;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> item)
    {
        UpdateInventoryUI?.Invoke(location, item);
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }

    public static event Action<int, Vector3, ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    public static event Action<ItemDetails, bool> ItemSelectEvent;
    public static void CallItemSelectEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int, int, int, Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int second, int minute, int day, Season season)
    {
        GameMinuteEvent?.Invoke(second, minute, day, season);
    }

    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    public static event Action<int, int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 position)
    {
        TransitionEvent?.Invoke(sceneName, position);
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

    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    public static event Action<Vector3, ItemDetails> ExcuteActionAfterAnimation;
    public static void CallExcuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExcuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(ID, tileDetails);
    }

    public static event Action<int> HaverstAtPlayerPosition;
    public static void CallHaverstAtPlayerPosition(int ID)
    {
        HaverstAtPlayerPosition?.Invoke(ID);
    }

    public static event Action RefreshCurrnetMap;
    public static void CallRefreshCurrnetMap()
    {
        RefreshCurrnetMap?.Invoke();
    }

    public static event Action<ParticalEffectType, Vector3> ParticalEffectEvent;
    public static void CallParticalEffectEvent(ParticalEffectType type, Vector3 pos)
    {
        ParticalEffectEvent?.Invoke(type, pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }

    public static event Action<SoltType, InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SoltType soltType, InventoryBag_SO data)
    {
        BaseBagOpenEvent?.Invoke(soltType, data);
    }
    public static event Action<SoltType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SoltType soltType, InventoryBag_SO data)
    {
        BaseBagCloseEvent?.Invoke(soltType, data);
    }

    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState State)
    {
        UpdateGameStateEvent?.Invoke(State);
    }

    public static event Action<ItemDetails, bool, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell, bool isSelected)
    {
        ShowTradeUI?.Invoke(item, isSell, isSelected);
    }

    /// <summary>
    /// 使用蓝图建造家具的方法
    /// </summary>
    public static event Action<int, Vector3> BuildFunitureEvent;
    public static void CallBuildFunitureEvent(int iD, Vector3 pos)
    {
        BuildFunitureEvent?.Invoke(iD, pos);
    }

    public static event Action<Season, LightShift, float> LightShiftChangeEvnet;
    public static void CallLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        LightShiftChangeEvnet?.Invoke(season, lightShift, timeDifference);
    }

    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails sound)
    {
        InitSoundEffect?.Invoke(sound);
    }

    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }

    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }

    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }

    public static event Action<QuestDetails> AcceptQuest;
    public static void CallAcceptQuest(QuestDetails questDetails)
    {
        AcceptQuest?.Invoke(questDetails);
    }

    public static event Action<QuestDetails> DeliveryQuestItems;
    public static void CallDeliveryQuestItems(QuestDetails questDetails)
    {
        DeliveryQuestItems?.Invoke(questDetails);
    }

    // public static event Action<QuestDetails> ShowQuestOnUI;
    // public static void CallShowQuestOnUI(QuestDetails questDetails)
    // {
    //     ShowQuestOnUI?.Invoke(questDetails);
    // }

    public static event Action<QuestDetails> ShowDetailOnInfoUI;
    public static void CallShowDetailOnInfoUI(QuestDetails questDetails)
    {
        ShowDetailOnInfoUI?.Invoke(questDetails);
    }

    public static event Action RefreshQuestDetails;
    public static void CallRefreshQuestDetails()
    {
        RefreshQuestDetails?.Invoke();
    }

}
