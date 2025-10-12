using System;
using System.Collections.Generic;
using Cinemachine;
using Farm.Save;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.春天;
    private int monthInSeason = 3;

    public bool gameClockPause;
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    public string GUID => GetComponent<DataGUID>().guid;

    private float tikTime;

    // 灯光时间差
    private float timeDifference;

    protected override void Awake()
    {
        base.Awake();
        NewGameTime();
    }

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();

        gameClockPause = true;

        // 同样是初始化，为什么放在Awake中？
        // 因为方法注册在OnEnable中，生命周期比Awake晚，会报空
        // EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        // EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        // // 灯光变化
        // EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }
    void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThresHold)
            {
                tikTime -= Settings.secondThresHold;
                UpdateGameTime();
            }
        }

        #region 作弊部分 仅限测试使用
        if (Input.GetKeyDown(KeyCode.T))
        {
            PassTenMinute();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PassOneMinute();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
        #endregion
    }

    public void PassTenMinute()
    {
        for (int i = 0; i < 600; i++)
            UpdateGameTime();
    }

    public void PassOneMinute()
    {
        for (int i = 0; i < 60; i++)
            UpdateGameTime();
    }

    private void OnAfterSceneLoadEvent()
    {
        gameClockPause = false;
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        // 灯光变化
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }

    private void OnUpdateGameStateEvent(GameState state)
    {
        gameClockPause = state == GameState.Pause;
    }

    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }

    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2025;
        gameSeason = Season.春天;
    }
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {

            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        gameMonth++;
                        gameDay = 1;

                        if (gameMonth > 12)
                            gameYear++;

                        monthInSeason--;
                        if (monthInSeason == 0)
                        {
                            monthInSeason = 3;

                            int seasonNumber = (int)gameSeason;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                gameYear++;
                                seasonNumber = 0;
                            }

                            if (gameYear > 2030)
                            {
                                // FIXME:可以适当添加彩蛋
                                gameYear = 2025;
                            }
                        }
                        EventHandler.CallGameDayEvent(gameDay, gameSeason);
                    }
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        }
        // Debug.Log("minute:" + gameMinute + "second:" + gameSecond);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    /// <summary>
    /// 获取当前的光照时间段
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime <= Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        if (GameTime < Settings.morningTime || GameTime > Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            return LightShift.Night;
        }
        return LightShift.Morning;
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();

        saveData.timeDict = new Dictionary<string, int>
        {
            { "gameYear", gameYear },
            { "gameSeason", (int)gameSeason },
            {"gameMonth",gameMonth},
            {"gameDay",gameDay},
            {"gameHour",gameHour},
            {"gameMinute",gameMinute},
            {"gameSecond",gameSecond}
        };

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        gameYear = saveData.timeDict["gameYear"];
        gameSeason = (Season)saveData.timeDict["gameSeason"];
        gameMonth = saveData.timeDict["gameMonth"];
        gameDay = saveData.timeDict["gameDay"];
        gameHour = saveData.timeDict["gameHour"];
        gameMinute = saveData.timeDict["gameMinute"];
        gameSecond = saveData.timeDict["gameSecond"];
    }
}
