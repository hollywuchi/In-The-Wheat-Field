using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;
        [Header("拖拽图片")]
        public Image dragItem;
        [Header("玩家背包UI")]
        [SerializeField] private GameObject playerBag;
        private bool openedUI;

        [Header("通用背包UI")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        public GameObject boxSlotPrefab;

        [Header("交易UI")]
        public TradeUI TradeUI;
        public TextMeshProUGUI playerMoneyText;
        [SerializeField] private SlotUI[] slotUIs;
        [SerializeField] private List<SlotUI> baseBagSlots;
        void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInvntoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }
        void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInvntoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }


        void Start()
        {
            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Index = i;
            }
            // 注意，现在activeInHierarchy已经代替了active
            openedUI = playerBag.activeInHierarchy;

            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();

        }

        void Update()
        {
            if (openedUI && Input.GetKeyDown(KeyCode.Escape))
            {
                playerBag.SetActive(false);
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            SwitchHighLight(-1);
        }


        /// <summary>
        /// 打开背包
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagOpenEvent(SoltType slotType, InventoryBag_SO bagData)
        {
            // WORKFLOW:通用的prefab
            GameObject prefab = slotType switch
            {
                SoltType.Shop => shopSlotPrefab,
                SoltType.Box => boxSlotPrefab,
                _ => null,
            };

            // 生成背包UI
            baseBag.SetActive(true);

            baseBagSlots = new List<SlotUI>();
            for (int i = 0; i < bagData.BagItemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.Index = i;
                baseBagSlots.Add(slot);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());

            if (slotType == SoltType.Shop)
            {
                // 修改背包UI的中心点
                playerBag.GetComponent<RectTransform>().pivot = new Vector2(-0.5f, 0.5f);
                playerBag.SetActive(true);
                openedUI = true;
            }
            // 刷新UI显示
            OnUpdateInvntoryUI(InventoryLocation.Box, bagData.BagItemList);
        }

        /// <summary>
        /// 关闭背包
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sO"></param>
        private void OnBaseBagCloseEvent(SoltType slotType, InventoryBag_SO sO)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);    // 与ESC会发生冲突
            SwitchHighLight(-1);    // 更新UI高亮显示

            foreach (var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SoltType.Shop)
            {
                // bug修复，在关闭背包之后更改背包的中心点
                playerBag.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                playerBag.SetActive(false);
                openedUI = false;
            }
        }

        private void OnShowTradeUI(ItemDetails details, bool isSell, bool isSelected)
        {
            TradeUI.gameObject.SetActive(isSelected);
            TradeUI.SetupTradeUI(details, isSell);
            if(!isSell)
                TradeUI.tradeText.text = "想要几个？";
            else
                TradeUI.tradeText.text = "要卖几个？";
        }

        /// <summary>
        /// 更新指定位置的UI事件
        /// </summary>
        /// <param name="location"></param>
        /// <param name="list"></param>
        private void OnUpdateInvntoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < slotUIs.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetDetails(list[i].itemID);
                            slotUIs[i].UpdateSolt(item, list[i].itemAmount);
                        }
                        else
                        {
                            slotUIs[i].UpdateEmptySolt();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSolt(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySolt();
                        }
                    }
                    break;
            }
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }
        /// <summary>
        /// 控制背包UI的打开与关闭,需要改进
        /// </summary>
        public void SwitchUIOpened()
        {
            openedUI = !openedUI;
            playerBag.SetActive(openedUI);
            if (openedUI)
                EventHandler.CallUpdateGameStateEvent(GameState.Pause);
            else
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
        }
        /// <summary>
        /// 转换格子UI周围的环绕动画
        /// </summary>
        /// <param name="index"></param>
        public void SwitchHighLight(int index)
        {
            // 数组合并
            var combineUI = slotUIs.Concat(baseBagSlots);

            foreach (var slot in combineUI)
            {
                if (slot.Index == index && slot.isSelected)
                {
                    slot.highLightImg.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.highLightImg.gameObject.SetActive(false);
                }
            }
            // 为什么点选商店中的UI不会显示周围动画？
            // 因为foreach中并没有遍历商店中的UI组也就是baseBagSlots
        }
    }
}
