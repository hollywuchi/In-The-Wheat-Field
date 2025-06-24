using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    }
    void OnDisable()
    {
        EventHandler.ItemSelectEvent -= OnItemSelectEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnItemSelectEvent(ItemDetails details, bool isSelected)
    {
        // C# 新的语法糖
        PartType currentType = details.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commondity => PartType.Carry,
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
                holdItem.sprite = details.itemOnWorldSprite;
                holdItem.enabled = true;
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
