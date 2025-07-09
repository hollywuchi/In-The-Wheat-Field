using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Farm.Map;
using System.Collections;
using Unity.Mathematics;
using UnityEditor.U2D.Aseprite;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    // 鼠标检测
    private Camera mianCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionVaild;

    private ItemDetails currentItem;

    private Transform playerPos;
    void OnEnable()
    {
        EventHandler.ItemSelectEvent += OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }


    void Start()
    {
        cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);
        mianCamera = Camera.main;
        playerPos = FindAnyObjectByType<Player>().transform;
    }

    void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
            SetCursorImage(normal);

    }
    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorVaild()
    {
        cursorPositionVaild = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorInVaild()
    {
        cursorPositionVaild = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
    }

    #endregion
    private void OnItemSelectEvent(ItemDetails details, bool isSelect)
    {
        if (!isSelect)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {
            currentItem = details;
            currentSprite = details.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commondity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.ReapTool => tool,
                ItemType.BreakTool => tool,
                ItemType.CollectTool => tool,
                ItemType.WaterTool => tool,
                _ => normal
            };
            cursorEnable = true;
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionVaild)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
    private void CheckCursorValid()
    {
        mouseWorldPos = mianCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mianCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(playerPos.position);

        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInVaild();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            // WORKFLOW:补充所有物品的类型
            switch (currentItem.itemType)
            {
                case ItemType.Commondity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorVaild(); else SetCursorInVaild();
                    break;

                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorVaild(); else SetCursorInVaild();
                    break;

                case ItemType.WaterTool:
                    if (currentTile.daysSinceDig > -1 && currentTile.daysSinceWatered == -1) SetCursorVaild(); else SetCursorInVaild();
                    break;
            }
        }
        else
        {
            SetCursorInVaild();
        }

    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        return false;
    }
}
