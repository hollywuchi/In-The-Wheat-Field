using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Save
{
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;
        public Dictionary<string, SerialzableVector3> characterPosDict;      // 人物位置字典（名字+位置）
        public Dictionary<string, List<SceneItem>> sceneItemDict;            // 场景物品字典（场景民称+物品列表）
        public Dictionary<string, List<SceneFuniture>> sceneFunitureDict;    // 场景家具字典（场景名称+家具列表）
        public Dictionary<string, TileDetails> tileDetailsDict;              // 场景瓦片字典（场景名称+位置信息+瓦片信息）
        public Dictionary<string, bool> firstLoadDict;                       // 场景首次加载字典（场景名称+bool标记）
        public Dictionary<string, List<InventoryItem>> inventoryDict;        // 各项背包物品字典（背包名称+物品列表）
        public Dictionary<string, int> timeDict;                             // 时间字典（日/月/年三个字典+对应时间）

        public int playerMoney;                                              // 玩家金钱

        // NPC部分
        public string targetScene;                                           // NPC目标场景，前面有了初始场景
        public bool interactable;                                            // NPC是否可以互动
        public int animationInstaceID;                                       // 动画片段的实例化ID，可以反推动画片段
    }
}
