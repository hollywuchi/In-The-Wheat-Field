using System.Collections.Generic;
using Farm.CropPlant;
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

        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

        private Grid currentGrid;
        private List<ReapItem> ItemsInRadius;
        private Season currentSeason;

        void OnEnable()
        {
            EventHandler.ExcuteActionAfterAnimation += OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrnetMap += RefreshMap;
        }
        void OnDisable()
        {
            EventHandler.ExcuteActionAfterAnimation -= OnExcuteActionAfterAnimation;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrnetMap -= RefreshMap;
        }



        void Start()
        {
            foreach (var mapData in mapDataList)
            {
                firstLoadDict.Add(mapData.sceneName, true);
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
        public TileDetails GetTileDetails(string key)
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
            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                // 为了保证树不被刷掉，提前保存ID
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
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
                Crop currnetCrop = GetCropObject(mouseWorldPos);
                // WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, ItemType.Seed);
                        break;
                    case ItemType.Commondity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, ItemType.Commondity);
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
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        // 三目运算符为了防止点击非树木位置产生报错
                        currnetCrop?.ProcessToolAction(itemDetails, currnetCrop.tile);
                        break;
                    case ItemType.CollectTool:
                        currnetCrop.ProcessToolAction(itemDetails, currentTile);
                        break;
                    case ItemType.ReapTool:
                        int Count = 0;
                        for (int i = 0; i < ItemsInRadius.Count; i++)
                        {
                            EventHandler.CallParticalEffectEvent(ParticalEffectType.ReapableScenery,ItemsInRadius[i].transform.position + Vector3.up);
                            ItemsInRadius[i].SpawnHarvestItems();
                            Destroy(ItemsInRadius[i].gameObject);
                            Count++;
                            if(Count > Settings.reapCount)
                                break;
                        }
                        break;
                }

                UpdateTileDetails(currentTile);
            }
        }

        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }
        /// <summary>
        /// 返回工具范围内的杂草
        /// </summary>
        /// <param name="tool">工具详情</param>
        /// <returns></returns>
        public bool HaveReapableItemsInReadius(Vector3 mouseWirldPos,ItemDetails tool)
        {
            ItemsInRadius = new List<ReapItem>();
            Collider2D[] colliders = new Collider2D[20];

            // 这个方法用于返回圆形范围中碰撞器，但是效率更高，性能更好
            Physics2D.OverlapCircleNonAlloc(mouseWirldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            ItemsInRadius.Add(item);
                        }
                    }

                }
            }
            return ItemsInRadius.Count > 0;

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
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.girdX + "X" + tileDetails.girdY + "Y" + SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
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

        /// <summary>
        /// 根据场景名称构建网格范围，输出范围和原点
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="gridDimensions">网格范围</param>
        /// <param name="girdOrigin">网格原点</param>
        /// <returns>是否有当前场景的信息</returns>
        public bool GetGridDemensions(string sceneName, out Vector2Int gridDimensions,out Vector2Int girdOrigin)
        {
            gridDimensions = Vector2Int.zero;
            girdOrigin = Vector2Int.zero;

            foreach(var mapdata in mapDataList)
            {
                if(mapdata.sceneName == sceneName)
                {
                    gridDimensions.x = mapdata.gridWitch;
                    gridDimensions.y = mapdata.gridHeight;

                    girdOrigin.x = mapdata.originX;
                    girdOrigin.y = mapdata.originY;

                    return true;
                }
            }
            return false;

        }
    }
}
