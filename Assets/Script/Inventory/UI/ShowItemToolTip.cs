using UnityEngine;
using UnityEngine.EventSystems;
namespace Farm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        void Awake()
        {
            slotUI = GetComponent<SlotUI>();    // 减少性能消耗
            
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null) // 跟进SlotUI中的修改
            {
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetUpToolTip(slotUI.itemDetails, slotUI.soltType);

                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }
    }
}
