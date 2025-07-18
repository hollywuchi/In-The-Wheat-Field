using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;
        void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        void Update()
        {
            if(Input.GetKeyDown(key))
            {
                if(slotUI.itemDetails != null)
                {
                    slotUI.isSelected = !slotUI.isSelected;
                    if(slotUI.isSelected)
                        slotUI.inventoryUI.SwitchHighLight(slotUI.Index);
                    else
                        slotUI.inventoryUI.SwitchHighLight(-1);

                    EventHandler.CallItemSelectEvent(slotUI.itemDetails,slotUI.isSelected);
                }
            }
        }
    }
}
