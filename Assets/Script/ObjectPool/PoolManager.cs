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
    private Queue<GameObject> soundQueue = new Queue<GameObject>();     // 队列，当做音效的对象池

    void OnEnable()
    {
        EventHandler.ParticalEffectEvent += OnParticalEffectEvent;
        EventHandler.InitSoundEffect += OnInitSoundEffect;
    }

    void OnDisable()
    {
        EventHandler.ParticalEffectEvent -= OnParticalEffectEvent;
        EventHandler.InitSoundEffect -= OnInitSoundEffect;
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

    // private void InitSoundEffect(SoundDetails soundDetails)
    // {
    //     ObjectPool<GameObject> pool = poolEffectList[4];
    //     var obj = pool.Get();

    //     obj.GetComponent<Sound>().SetSound(soundDetails);
    //     StartCoroutine(DisableSound(pool, obj, soundDetails));
    // }

    // private IEnumerator DisableSound(ObjectPool<GameObject> pool, GameObject obj, SoundDetails soundDetails)
    // {
    //     yield return new WaitForSeconds(soundDetails.soundClip.length);
    //     pool.Release(obj);
    // }

    /// <summary>
    /// 创建并初始化声音对象池
    /// </summary>
    private void CreatSoundPool()
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }

    /// <summary>
    /// 获取对象池中的对象
    /// </summary>
    /// <returns></returns>
    private GameObject GetPoolProgect()
    {
        if(soundQueue.Count < 2)
            CreatSoundPool();
        return soundQueue.Dequeue();
    }

    private void OnInitSoundEffect(SoundDetails details)
    {
        var obj = GetPoolProgect();
        obj.GetComponent<Sound>().SetSound(details);
        obj.SetActive(true);

        StartCoroutine(DisableSound(obj,details.soundClip.length));
    }

    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }
}
