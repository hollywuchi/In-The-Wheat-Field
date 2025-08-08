using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;
        [Header("玩家背包UI")]
        [SerializeField] private GameObject playerBag;
        public Image dragItem;
        private bool openedUI;

        [Header("通用背包UI")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;
        [SerializeField] private SlotUI[] slotUIs;
        [SerializeField] private List<SlotUI> baseBagSlots;
        void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInvntoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }
        void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInvntoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;

        }


        void Start()
        {
            for (int i = 0; i < slotUIs.Length; i++)
            {
                slotUIs[i].Index = i;
            }
            // 注意，现在activeInHierarchy已经代替了active
            openedUI = playerBag.activeInHierarchy;
        }

        private void OnBeforeSceneUnloadEvent()
        {
            SwitchHighLight(-1);
        }

        private void OnBaseBagOpenEvent(SoltType slotType, InventoryBag_SO bagData)
        {
            // TODO:通用的prefab
            GameObject prefab = slotType switch
            {
                SoltType.Shop => shopSlotPrefab,
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
            // 刷新UI显示
            OnUpdateInvntoryUI(InventoryLocation.Box, bagData.BagItemList);
        }

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
        }
        /// <summary>
        /// 控制背包UI的打开与关闭
        /// </summary>
        public void SwitchUIOpened()
        {
            openedUI = !openedUI;
            playerBag.SetActive(openedUI);
        }

        public void SwitchHighLight(int index)
        {
            foreach (var slot in slotUIs)
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
        }



    }
}
