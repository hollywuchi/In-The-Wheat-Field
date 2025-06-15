using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    // TODO:场景转换优化
    void Start()
    {
        SwitchBoundsInChange();
    }
    
    void SwitchBoundsInChange()
    {
        PolygonCollider2D Bounds = GameObject.FindGameObjectWithTag("Bound").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = Bounds;

        // 转换边界之后清除缓存
        confiner.InvalidatePathCache();
    }
}
