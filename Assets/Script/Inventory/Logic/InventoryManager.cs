using System.Collections.Generic;
using Farm.Save;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

// 引用了命名空间，其他脚本中就没办法直接调用InventoryManager，除非调用这个命名空间
// 为了防止乱调用，产生的耦合，也方便之后解耦所用
namespace Farm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>, ISaveable
    {
        [Header("物品数据库")]
        public ItemDetailList_SO itemLibrary;
        [Header("图纸数据库")]
        public BluePrintDataList_SO bluePrintLibrary;
        [Header("背包数据库")]
        public InventoryBag_SO playerBagTemp;
        public InventoryBag_SO playerBag;
        private InventoryBag_SO currentBoxBag;

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;

        public string GUID => GetComponent<DataGUID>().guid;

        [Header("交易")]
        public int playerMoney;

        void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HaverstAtPlayerPosition += OnHaverstAtPlayerPosition;
            EventHandler.BuildFunitureEvent += OnBuildFunitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HaverstAtPlayerPosition -= OnHaverstAtPlayerPosition;
            EventHandler.BuildFunitureEvent -= OnBuildFunitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }



        void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
            // EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
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
        public bool CheckBagCapacity()
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
            if (Index == -1 && CheckBagCapacity())  //背包中没有物品且背包还有容量
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

        /// <summary>
        /// 背包和其他容器之间的物品交换
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="loactionTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation loactionTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(loactionTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)   // 交换两个不相同的物品
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (targetItem.itemID == currentItem.itemID)   // 两个相同的物品直接堆叠
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    // 目标为空
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }

                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(loactionTarget, targetList);
            }
        }

        /// <summary>
        /// 根据位置返回背包数据列表
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.BagItemList,
                InventoryLocation.Box => currentBoxBag.BagItemList,
                _ => null
            };
        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        /// <summary>
        /// 添加收获的农作物到背包（直接添加版）
        /// </summary>
        /// <param name="ID">农作物ID</param>
        private void OnHaverstAtPlayerPosition(int ID)
        {
            var Index = GetItemIndexInBag(ID);

            AddItemByIndex(ID, Index, 1);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }

        private void OnBuildFunitureEvent(int ID, Vector3 pos)
        {
            RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintLibrary.GetBluePrint(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }

        private void OnBaseBagOpenEvent(SoltType soltType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }


        private void OnStartNewGameEvent(int obj)
        {
            playerBag = Instantiate(playerBagTemp);
            playerMoney = Settings.playerStartMoney;
            boxDataDict.Clear();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }

        /// <summary>
        /// 移除指定物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">数量</param>
        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if (playerBag.BagItemList[index].itemAmount > removeAmount)
            {
                var amount = playerBag.BagItemList[index].itemAmount - removeAmount;
                InventoryItem newItem = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.BagItemList[index] = newItem;
            }
            else if (playerBag.BagItemList[index].itemAmount == removeAmount)
            {
                InventoryItem newItem = new InventoryItem();
                playerBag.BagItemList[index] = newItem;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }

        /// <summary>
        /// 买卖方法
        /// </summary>
        /// <param name="itemDetails">确认物品</param>
        /// <param name="amount">买卖数目</param>
        /// <param name="isSellTrade">买还是卖</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)     // 卖
            {
                if (playerBag.BagItemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    // 售卖总价
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost > 0)     // 买
            {
                if (CheckBagCapacity())
                {
                    AddItemByIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;

            }

            // 刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);
        }

        /// <summary>
        /// 检查建造资源库存
        /// </summary>
        /// <param name="ID">蓝图ID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetils = bluePrintLibrary.GetBluePrint(ID);

            foreach (var resourceItem in bluePrintDetils.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 从字典中获取对应箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }

        /// <summary>
        /// 将箱子数据传到字典
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataList(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
                boxDataDict.Add(key, box.boxBagData.BagItemList);
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = playerMoney;

            saveData.inventoryDict = new Dictionary<string, List<InventoryItem>>()
            {
                {playerBag.name,playerBag.BagItemList}
            };

            foreach (var item in boxDataDict)
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.playerMoney = saveData.playerMoney;
            playerBag = Instantiate(playerBagTemp);
            playerBag.BagItemList = saveData.inventoryDict[playerBag.name];

            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.BagItemList);

            // FIXME:注意视频中所说的缺少Playermoney
        }
    }
}
