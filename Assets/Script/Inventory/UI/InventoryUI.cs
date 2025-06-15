using System.Collections.Generic;
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
        [SerializeField] private SlotUI[] slotUIs;
        void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInvntoryUI;
        }
        void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInvntoryUI;
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
            foreach(var slot in slotUIs)
            {
                if(slot.Index ==  index && slot.isSelected)
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
