using UnityEngine;

// 引用了命名空间，其他脚本中就没办法直接调用InventoryManager，除非调用这个命名空间
// 为了防止乱调用，产生的耦合，也方便之后解耦所用
namespace Farm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据库")]
        public ItemDetailList_SO itemLibrary;
        [Header("背包数据库")]
        public InventoryBag_SO playerBag;

        void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }

        /// <summary>
        /// 返回数据库中的物品
        /// </summary>
        /// <param name="ID">物品的ID</param>
        /// <returns></returns>
        public ItemDetails GetDetails(int ID)
        {
            return itemLibrary.itemDetailsList.Find(i => i.itemID == ID);
        }

        public InventoryItem getitem(int ID)
        {
            return playerBag.BagItemList.Find(i => i.itemID == ID);
        }

        public void AddItem(Item item, bool toDestory)
        {
            var Index = GetItemIndexInBag(item.itemID);

            AddItemByIndex(item.itemID, Index, 1);

            if (toDestory)
                Destroy(item.gameObject);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }
        /// <summary>
        /// 确认背包是否有空位
        /// </summary>
        /// <returns></returns>
        public bool ChackBagCapacity()
        {
            for (int i = 0; i < playerBag.BagItemList.Count; i++)
            {
                if (playerBag.BagItemList[i].itemID == 0)
                    return true;
            }
            // 为什么不写elseif，因为上面已经return，程序运行不到这里
            return false;
        }
        /// <summary>
        /// 查找背包中是否有该物品
        /// </summary>
        /// <param name="ID">物品序号</param>
        /// <returns>返回物品在背包的第几个格子，-1为没有该物品</returns>
        public int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.BagItemList.Count; i++)
            {
                if (playerBag.BagItemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        private void AddItemByIndex(int ID, int Index, int Amount)
        {
            if (Index == -1 && ChackBagCapacity())  //背包中没有物品且背包还有容量
            {
                for (int i = 0; i < playerBag.BagItemList.Count; i++)
                {
                    if (playerBag.BagItemList[i].itemID == 0)
                    {
                        var newItem = new InventoryItem { itemID = ID, itemAmount = Amount };
                        playerBag.BagItemList[i] = newItem;
                        break;
                    }
                }
            }
            else    // 背包中有这个物品
            {
                // 加上这个物品的数量
                int currentAmount = playerBag.BagItemList[Index].itemAmount + Amount;
                var newItem = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.BagItemList[Index] = newItem;
            }
        }
        /// <summary>
        /// 交换背包中两个物品的位置（拖动）
        /// </summary>
        /// <param name="formSoltIndex"></param>
        /// <param name="targetSoltIndex"></param>
        public void SwapItem(int formSoltIndex, int targetSoltIndex)
        {
            var fromSolt = playerBag.BagItemList[formSoltIndex];
            var targetSolt = playerBag.BagItemList[targetSoltIndex];

            if (targetSolt.itemID == 0)
            {
                playerBag.BagItemList[targetSoltIndex] = fromSolt;
                playerBag.BagItemList[formSoltIndex] = new InventoryItem();
            }
            else
            {
                playerBag.BagItemList[targetSoltIndex] = fromSolt;
                playerBag.BagItemList[formSoltIndex] = targetSolt;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }

    }

}
