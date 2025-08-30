using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteData;
    public List<NPCPosition> npcPositionList;

    private Dictionary<string, SceneRoute> sceneRouteDic = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDic();
    }

    void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;

    }

    void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        foreach (var character in npcPositionList)
        {
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().StartScene = character.startScene;
        }
    }

    /// <summary>
    /// 初始化路径字典
    /// </summary>
    private void InitSceneRouteDic()
    {
        if (sceneRouteData.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteData.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDic.ContainsKey(key))
                    continue;
                else
                    sceneRouteDic.Add(key, route);
            }
        }
    }


    /// <summary>
    /// 获得两个场景间的路径
    /// </summary>
    /// <param name="fromSceneName"></param>
    /// <param name="gotoSceneName"></param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDic[fromSceneName + gotoSceneName];
    }
}
