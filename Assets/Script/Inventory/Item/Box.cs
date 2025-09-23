using System.Collections;
using System.Collections.Generic;
using Farm.Inventory;
using UnityEditor.TerrainTools;
using UnityEngine;

public class Box : MonoBehaviour
{
    public InventoryBag_SO boxBagTemplete;
    public InventoryBag_SO boxBagData;

    public GameObject mouseIcon;
    private bool isOpen = false;
    private bool canOpen;

    public int index;

    void OnEnable()
    {
        if (boxBagData == null)
        {
            boxBagData = Instantiate(boxBagTemplete);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpen = true;
            mouseIcon.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpen = false;
            mouseIcon.SetActive(false);
        }
    }

    void Update()
    {
        if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
        {
            isOpen = true;
            EventHandler.CallBaseBagOpenEvent(SoltType.Box, boxBagData);
        }

        if (isOpen && !canOpen)
        {
            isOpen = false;
            EventHandler.CallBaseBagCloseEvent(SoltType.Box, boxBagData);
        }

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = false;
            EventHandler.CallBaseBagCloseEvent(SoltType.Box, boxBagData);
        }
    }

    public void InitBox(int boxIndex)
    {
        index = boxIndex;
        var key = this.name + index;
        if (InventoryManager.Instance.GetBoxDataList(key) != null)
        {
            boxBagData.BagItemList = InventoryManager.Instance.GetBoxDataList(key);
        }
        else
        {
            InventoryManager.Instance.AddBoxDataList(this);
        }
    }
}
