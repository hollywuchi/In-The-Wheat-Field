using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Farm.CropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataList_SO cropDataBase;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;


        void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }

        void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }

        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }

        private void OnAfterSceneLoadEvent()
        {
            currentGrid = FindAnyObjectByType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnPlantSeedEvent(int ID, TileDetails details)
        {
            CropDetails currentCrop = GetCropDetails(ID);
            if (currentCrop != null && SeasonAvailable(currentCrop) && details.seedItemID == -1) // 用于第一次种植
            {
                details.seedItemID = ID;
                details.growthDays = 0;
                // 显示农作物
                DisplayCropPlant(details,currentCrop);
            }
            else if (details.seedItemID != -1) // 用于刷新农作物
            {
                // 显示农作物
                DisplayCropPlant(details,currentCrop);
            }
        }

        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails">瓦片地图信息</param>
        /// <param name="cropDetails">种子信息</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            int growthStage = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;   // 种子从种下到成熟所需的总天数

            // 计算当前的成长阶段
            for (int i = growthStage - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];    // 相减之后获得当前的天数
            }

            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            Vector3 pos = new Vector3(tileDetails.girdX + 0.5f, tileDetails.girdY + 0.5f);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);

            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
        }

        /// <summary>
        /// 根据物品ID在种子数据库中查找对应种子
        /// </summary>
        /// <param name="ID">种子ID</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropDataBase.cropDetailsList.Find(c => c.seedItemID == ID);
        }

        /// <summary>
        /// 判断种子是否可以在当前季节被种植
        /// </summary>
        /// <param name="crop">种子信息</param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for (int i = 0; i < crop.seasons.Length; i++)
            {
                if (crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }

}
