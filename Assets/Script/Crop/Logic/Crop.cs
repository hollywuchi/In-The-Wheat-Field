using System.Collections;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tile;
    public bool canHarvest => tile.growthDays > cropDetails.TotalGrowthDays;
    private int harvestActionCount;
    public Animator anim;
    private Transform PlayerTrans => FindAnyObjectByType<Player>().transform;
    public void ProcessToolAction(ItemDetails tool, TileDetails tileDetails)
    {
        tile = tileDetails;
        // 获取工具使用次数
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;


        anim = GetComponentInChildren<Animator>();
        // 点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            if (anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTrans.position.x < transform.position.x)
                    anim.SetTrigger("RotateRight");
                else
                    anim.SetTrigger("RotateLeft");
            }

            // 判断是否有动画
            // 播放音效
            // 播放动画
        }
        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
            {
                // 生成农作物
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAnimation)
            {
                if (PlayerTrans.position.x < transform.position.x)
                    anim.SetTrigger("FallingRight");
                else
                    anim.SetTrigger("FallingLeft");

                StartCoroutine(HarvestAfterAnimation());
            }
        }

    }

    private IEnumerator HarvestAfterAnimation()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return null;
        }

        SpawnHarvestItems();

        // 可能会执行,转换新物品
        if (cropDetails.transferItemID > 0)
        {
            CreatTransferCrop();
        }


    }


    private void CreatTransferCrop()
    {
        tile.seedItemID = cropDetails.transferItemID;
        tile.daysSinceLastHarvest = -1;
        // BUG:会使得鼠标检测失效
        // tile.growthDays = 0;

        EventHandler.CallRefreshCurrnetMap();
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

        if (tile != null)
        {
            tile.daysSinceLastHarvest++;

            // 判断是否可以生长
            if (tile.daysSinceLastHarvest < cropDetails.RegrowTimes && cropDetails.daysToRegrow > 0)
            {
                tile.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                // 刷新种子
                EventHandler.CallRefreshCurrnetMap();
            }
            else  // 不可重复生长
            {
                tile.daysSinceLastHarvest = -1;
                tile.seedItemID = -1;
            }

            Destroy(gameObject);
        }
    }

}
