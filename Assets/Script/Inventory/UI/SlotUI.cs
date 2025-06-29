using DG.Tweening;
using Farm.Inventory;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("获取组件")]

    [SerializeField] private Image slotImg;
    [SerializeField] private TextMeshProUGUI amountText;
    public Image highLightImg;
    [SerializeField] private Button button;
    private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
    [Header("格子类型")]
    public SoltType soltType;
    public bool isSelected;

    public ItemDetails itemDetails;
    public int itemAmount;
    public int Index;


    void Start()
    {
        isSelected = false;
        if (itemDetails == null)
        {
            UpdateEmptySolt();
        }
    }

    /// <summary>
    /// 显示对应格子中的物品
    /// </summary>
    /// <param name="item">物品实例ItemDetails</param>
    /// <param name="amount">物品数量</param>
    public void UpdateSolt(ItemDetails item, int amount)
    {
        itemDetails = item;

        slotImg.sprite = item.itemIcon;
        itemAmount = amount;
        amountText.text = amount.ToString();
        slotImg.enabled = true;
        button.interactable = true;
    }
    /// <summary>
    /// 将格子变空
    /// </summary>
    public void UpdateEmptySolt()
    {
        if (isSelected)
        {
            isSelected = false;

            inventoryUI.SwitchHighLight(-1);
            EventHandler.CallItemSelectEvent(itemDetails, isSelected);
        }
        // 修改这里会牵扯到很多代码，虽然其中的amount是0，但是不允许返回值，此为空指针
        itemDetails = null;
        slotImg.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemDetails == null) return;

        isSelected = !isSelected;

        inventoryUI.SwitchHighLight(Index);
        // 判断是否点按的是背包的物品
        // 否则点按超市的物品直接零元购
        if (soltType == SoltType.Bag)
        {
            // 点按通知转换姿势(物品的信息和是否点选)
            EventHandler.CallItemSelectEvent(itemDetails, isSelected);
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemAmount != 0)
        {
            inventoryUI.dragItem.enabled = true;
            inventoryUI.dragItem.sprite = slotImg.sprite;
            // inventoryUI.dragItem.SetNativeSize();   // 还原成默认大小
            // 适应大格子，统一变成1.5倍
            float newWidth = (float)inventoryUI.dragItem.sprite.rect.width * 1.5f;
            float newHeight = (float)inventoryUI.dragItem.sprite.rect.height * 1.5f;
            inventoryUI.dragItem.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);

            isSelected = true;
            inventoryUI.SwitchHighLight(Index);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.dragItem.enabled = false;
        // isSelected = false;
        // inventoryUI.SwitchHighLight(Index);

        // Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                return;

            var targetSolt = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            int targetSoltIndex = targetSolt.Index;

            if (soltType == SoltType.Bag && targetSolt.soltType == SoltType.Bag)
            {
                InventoryManager.Instance.SwapItem(Index, targetSoltIndex);
            }

            inventoryUI.SwitchHighLight(-1);
        }
        else    // 尝试把物品丢到地上
        {
            if (itemDetails.canDropped)
            {
                // 将鼠标坐标映射到世界地图上
                var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

                EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            }
        }
    }
}
