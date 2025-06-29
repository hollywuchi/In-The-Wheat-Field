using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("地图数据")]
        public List<MapData_SO> mapDataList;

        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        private Grid currentGrid;

        void OnEnable()
        {
            EventHandler.ExcuteActionAfterAnimation += OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }
        void OnDisable()
        {
            EventHandler.ExcuteActionAfterAnimation -= OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }



        void Start()
        {
            foreach (var mapData in mapDataList)
            {
                InitTileDetailsDict(mapData);
            }
        }

        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty property in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    girdX = property.tileCoordinate.x,
                    girdY = property.tileCoordinate.y
                };

                string key = tileDetails.girdX + "X" + tileDetails.girdY + "Y" + mapData.sceneName;


                // 在修改场景之后，tileDetail中的信息可能会发生改变，字典也应该刷新，刷新之前要先尝试获取这其中的值
                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (property.girdType)
                {
                    case GirdType.Diggable:
                        tileDetails.canDig = property.boolTypeValue;
                        break;
                    case GirdType.DropItem:
                        tileDetails.canDropItem = property.boolTypeValue;
                        break;
                    case GirdType.NPCObstacle:
                        tileDetails.isNPCObstacle = property.boolTypeValue;
                        break;
                    case GirdType.PlaceFurinture:
                        tileDetails.canPlaceFunture = property.boolTypeValue;
                        break;
                }

                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;
                }
                else
                    tileDetailsDict.Add(key, tileDetails);

            }
        }

        /// <summary>
        /// 查找字典中有没有这个键
        /// </summary>
        /// <param name="key">坐标X+坐标Y+场景名称</param>
        /// <returns></returns>
        private TileDetails GetTileDetails(string key)
        {
            if (tileDetailsDict.ContainsKey(key))
                return tileDetailsDict[key];
            else
                return null;
        }

        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "X" + mouseGridPos.y + "Y" + SceneManager.GetActiveScene().name;
            return GetTileDetails(key);
        }

        /// <summary>
        /// 执行实际工具或物品的功能
        /// </summary>
        /// <param name="mouseWorldPos">鼠标所在位置</param>
        /// <param name="itemDetails">物品信息</param>
        private void OnExcuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if (currentGrid != null)
            {
                switch (itemDetails.itemType)
                {
                    case ItemType.Commondity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos);
                        break;
                }
            }
        }
    }
}
