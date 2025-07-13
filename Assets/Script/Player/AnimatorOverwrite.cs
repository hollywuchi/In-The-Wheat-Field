using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Farm.Inventory;
using UnityEngine;

public class AnimatorOverwrite : MonoBehaviour
{
    private Animator[] animators;
    public SpriteRenderer holdItem;

    [Header("各部分动画列表")]
    public List<AnimatorTypes> animatorTypes;

    private Dictionary<string, Animator> animatorNameDic = new Dictionary<string, Animator>();

    void Awake()
    {
        // lambda表达式修正
        animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            animatorNameDic.Add(anim.name, anim);
        }
    }

    void OnEnable()
    {
        EventHandler.ItemSelectEvent += OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HaverstAtPlayerPosition += OnHaverstAtPlayerPosition;
    }
    void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HaverstAtPlayerPosition -= OnHaverstAtPlayerPosition;
    }

    
    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnHaverstAtPlayerPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetDetails(ID).itemOnWorldSprite;
        if(holdItem.enabled == false)
        {
            StartCoroutine(ShowItem(itemSprite));
        }

    }

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(0.5f);
        holdItem.enabled = false;
    }

    private void OnItemSelectEvent(ItemDetails details, bool isSelected)
    {
        // C# 新的语法糖
        PartType currentType = details.itemType switch
        {
            // WORKFLOW:物品使用时对应的动画
            ItemType.Seed => PartType.Carry,
            ItemType.Commondity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            _ => PartType.None
        };

        if (isSelected == false)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if (currentType == PartType.Carry)
            {
                holdItem.sprite = details.itemOnWorldSprite == null ? details.itemIcon : details.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else
            {
                // 切换其他工具之后手要放下来
                holdItem.enabled = false;
            }
        }
        SwitchAnimator(currentType);

    }

    public void SwitchAnimator(PartType currentType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == currentType)
            {
                // 字典发力！传入物品的类别名称，返回对应的动画类别
                animatorNameDic[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
