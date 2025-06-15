using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(SpriteRenderer))]
public class ItemFader : MonoBehaviour
{
    SpriteRenderer sprite => GetComponent<SpriteRenderer>();
    /// <summary>
    /// 变成半透明
    /// </summary>
    public void FadeIn()
    {
        Color targetColor = new Color(1,1,1,Settings.targetColor);
        sprite.DOColor(targetColor,Settings.fadeDuration);
        // DOColor(目标颜色，持续时间)
    }
    /// <summary>
    /// 变回正常颜色
    /// </summary>
    public void FadeOut()
    {
        Color targetColor = new Color(1,1,1,1);
        sprite.DOColor(targetColor,Settings.fadeDuration);
    }
}
