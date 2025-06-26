using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    void OnEnable()
    {
        EventHandler.ItemSelectEvent += OnItemSelectEvent;
    }

    void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectEvent;
    }


    void Start()
    {
        cursorCanvas = GameObject.FindWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);
    }

    void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI())
            SetCursorImage(currentSprite);
        else
            SetCursorImage(normal);

    }


    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void OnItemSelectEvent(ItemDetails details, bool isSelect)
    {
        if (!isSelect)
        {
            currentSprite = normal;
        }
        else
        {
            currentSprite = details.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commondity => item,
                ItemType.ChopTool => tool,
                _ => normal
            };
        }
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        return false;
    }
}
