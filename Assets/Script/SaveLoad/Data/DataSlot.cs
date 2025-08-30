using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Farm.Transition;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

namespace Farm.Save
{
    public class DataSlot
    {
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region  用来显示UI进度详情
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "年/" + (Season)timeData.timeDict["gameSeason"] + timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日";
                }
                else return string.Empty;
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var sceneData = dataDict[key];

                    return sceneData.dataSceneName switch
                    {
                        "00.Start" => "树林",
                        "01.Field" => "麦田",
                        "02.Home" => "小木屋",
                        "03.Town" => "城镇",
                        _ => string.Empty
                    };
                }
                else return string.Empty;
            }
        }
        #endregion
    }
}

