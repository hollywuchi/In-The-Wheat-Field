using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    // 这是对象池中的物品列表
    public List<GameObject> poolPrefabs;
    // 对象池的列表
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    void OnEnable()
    {
        EventHandler.ParticalEffectEvent += OnParticalEffectEvent;
    }

    void OnDisable()
    {
        EventHandler.ParticalEffectEvent -= OnParticalEffectEvent;
    }


    void Start()
    {
        CreatPool();
    }

    /// <summary>
    /// 生成对象池
    /// </summary>
    private void CreatPool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },    // e指得是所有物体
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticalEffectEvent(ParticalEffectType type, Vector3 pos)
    {
        // WORKFLOW:根据物品类型补充特效
        var objPool = type switch
        {
            ParticalEffectType.LeavesFalling01 => poolEffectList[0],
            ParticalEffectType.LeavesFalling02 => poolEffectList[1],
            ParticalEffectType.Rock => poolEffectList[2],
            ParticalEffectType.ReapableScenery => poolEffectList[3],
            _ => null
        };
        GameObject obj = objPool.Get();
        obj.transform.position = pos;

        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }
}
