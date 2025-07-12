using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Farm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        [Header("地图瓦片信息")]
        public RuleTile digTile;
        public RuleTile wetTile;
        private Tilemap digTileMaps;
        private Tilemap wetTileMaps;
        [Header("地图数据")]
        public List<MapData_SO> mapDataList;

        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        private Grid currentGrid;

        private Season currentSeason;

        void OnEnable()
        {
            EventHandler.ExcuteActionAfterAnimation += OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }
        void OnDisable()
        {
            EventHandler.ExcuteActionAfterAnimation -= OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
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
            digTileMaps = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            wetTileMaps = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            RefreshMap();
        }

        private void OnGameDayEvent(int days, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                    tile.Value.daysSinceWatered = -1;
                if (tile.Value.daysSinceDig > -1)
                    tile.Value.daysSinceDig++;

                // 过期的坑
                if (tile.Value.daysSinceDig > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDig = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1; 
                }
                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;

                }

            }

            RefreshMap();
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
                // WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID,mouseWorldPos,ItemType.Seed);
                        break;
                    case ItemType.Commondity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos,ItemType.Commondity);
                        break;

                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDig = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        // 音效
                        break;
                    case ItemType.WaterTool:
                        SetWetGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        // 音效
                        break;
                    case ItemType.CollectTool:
                        Crop currnetCrop = GetCropObject(mouseWorldPos);
                        // TODO:执行收割方法
                        break;
                }

                UpdateTileDetails(currentTile);
            }
        }

        private Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;

            for(int i = 0;i < colliders.Length; i++)
            {
                if(colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }
        /// <summary>
        /// 显示挖坑瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.girdY, 0);
            if (digTileMaps != null)
                digTileMaps.SetTile(pos, digTile);
        }
        /// <summary>
        /// 显示浇水瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWetGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.girdX, tile.girdY, 0);
            if (wetTileMaps != null)
                wetTileMaps.SetTile(pos, wetTile);
        }

        /// <summary>
        /// 更新地图中瓦片的信息
        /// </summary>
        /// <param name="tileDetails"></param>
        private void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "X" + tileDetails.girdY + "Y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
        }

        private void RefreshMap()
        {
            if (digTileMaps != null)
                digTileMaps.ClearAllTiles();
            if (wetTileMaps != null)
                wetTileMaps.ClearAllTiles();
            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName"></param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDig > -1)
                        SetDigGround(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWetGround(tileDetails);
                    if (tileDetails.seedItemID > -1)
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }
    }
}
