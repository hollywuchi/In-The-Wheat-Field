using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.春天;
    private int monthInSeason = 3;

    public bool gameClockPause;

    private float tikTime;

    void Awake()
    {
        NewGameTime();
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
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 1;
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

                            if (gameYear > 3)
                            {
                                // FIXME:可以适当添加彩蛋
                                gameYear = 2025;
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("minute:" + gameMinute + "second:" + gameSecond);

    }
}
