using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    // 这是对象池中的物品列表
    public List<GameObject> poolPrefabs;
    // 对象池的列表
    public List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
}
