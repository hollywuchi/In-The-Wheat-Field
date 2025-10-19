using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundsInChange;
    }
    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundsInChange;
    }
       
    void SwitchBoundsInChange()
    {
        PolygonCollider2D Bounds = GameObject.FindGameObjectWithTag("Bound").GetComponent<PolygonCollider2D>();

        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();

        confiner.m_BoundingShape2D = Bounds;

        // 转换边界之后清除缓存
        // confiner.InvalidatePathCache();
        confiner.InvalidateCache();
    }
}
