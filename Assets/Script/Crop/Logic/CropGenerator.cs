using Farm.Map;
using UnityEngine;

namespace Farm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        public int seedItemID;
        public int growthDays;

        private Grid currentGrid;

        void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }
        void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;

        }
        void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);
            if (seedItemID != 0)
            {
                TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);

                if (tile == null)
                {
                    tile = new TileDetails();
                    // bug:在没有瓦片信息的瓦片上种植时，会生成一片瓦片并添加到数据库。
                    // 但是没有给瓦片赋值的话就会在原点生成一个瓦片然后将作物种植在原点
                    tile.girdX = cropGridPos.x;
                    tile.girdY = cropGridPos.y;
                }

                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;
                tile.daysSinceWatered = -1;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}
