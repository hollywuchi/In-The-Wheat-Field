using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform PlayerTrans => FindObjectOfType<Player>().transform;

        public void InitCropDetails(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
        }
        /// <summary>
        /// 生成农作物
        /// </summary>
        public void SpawnHarvestItems()
        {
            // 循环掉落几种作物
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;

                if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
                    // 只生成一种农作物
                    amountToProduce = cropDetails.producedMinAmount[i];
                else
                    // Range中包括前一个，但是不包括最后一个数
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);

                for (int j = 0; j < amountToProduce; j++)
                {
                    // 真正的生成物品
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHaverstAtPlayerPosition(cropDetails.producedItemID[i]);
                    }
                    else    // 在世界上生成物品
                    {
                        var dirX = PlayerTrans.position.x < transform.position.x ? 1 : -1;

                        var spwanPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                        transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y));

                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spwanPos);
                    }

                }
            }
        }
    }
}
