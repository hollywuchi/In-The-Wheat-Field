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
                }

                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;
                tile.daysSinceWatered = -1;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}
