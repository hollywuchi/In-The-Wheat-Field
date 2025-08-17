using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Farm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;
        private Transform playerTans => FindAnyObjectByType<Player>().transform;
        // 记录场景物品
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        // 记录场景家具
        private Dictionary<string, List<SceneFuniture>> sceneFunitureDict = new Dictionary<string, List<SceneFuniture>>();
        void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BuildFunitureEvent += OnBuildFunitureEvent;
        }


        void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BuildFunitureEvent -= OnBuildFunitureEvent;
        }



        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFuniture();
        }

        private void OnAfterSceneLoadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreatSceneItems();
            RebuildFuniture();
        }

        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var newItem = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            newItem.itemID = ID;
            newItem.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
        }

        private void OnDropItemEvent(int ID, Vector3 mousePos, ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;

            var newItem = Instantiate(bounceItemPrefab, playerTans.position, Quaternion.identity, itemParent);
            newItem.itemID = ID;
            var dir = (mousePos - playerTans.position).normalized;

            newItem.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        private void OnBuildFunitureEvent(int ID, Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintLibrary.GetBluePrint(ID);
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);

            if (buildItem.GetComponent<Box>())
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);
            }
        }

        /// <summary>
        /// 获取当前场景中的物体
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();

            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerialzableVector3(item.transform.position)
                };

                currentSceneItem.Add(sceneItem);
            }

            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItem;
            }
            else
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItem);
            }
        }


        /// <summary>
        /// 重新生成场景中的物品
        /// </summary>
        private void RecreatSceneItems()
        {
            List<SceneItem> currnetSceneItem;

            // dict的函数，尝试从某处拿到key匹配然后更新value
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currnetSceneItem))
            {
                if (currnetSceneItem != null)
                {
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    // 从列表中拿到item的位置，实例化这些item
                    foreach (var item in currnetSceneItem)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.itemID = item.itemID;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前场景中的家具
        /// </summary>
        public void GetAllSceneFuniture()
        {
            List<SceneFuniture> currentSceneFuniture = new List<SceneFuniture>();

            foreach (var funiture in FindObjectsOfType<Funiture>())
            {
                SceneFuniture scenefuniture = new SceneFuniture
                {
                    itemID = funiture.itemID,
                    position = new SerialzableVector3(funiture.transform.position)
                };

                if (funiture.GetComponent<Box>())
                    scenefuniture.boxIndex = funiture.GetComponent<Box>().index;    // 保存箱子的序号

                currentSceneFuniture.Add(scenefuniture);
            }

            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneFunitureDict[SceneManager.GetActiveScene().name] = currentSceneFuniture;
            }
            else
            {
                sceneFunitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFuniture);
            }
        }

        /// <summary>
        /// 重新生成场景中的家具
        /// </summary>
        private void RebuildFuniture()
        {
            List<SceneFuniture> currentSceneFuniture = new List<SceneFuniture>();

            if (sceneFunitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFuniture))
            {
                if (currentSceneFuniture != null)
                {
                    foreach (SceneFuniture scenefuniture in currentSceneFuniture)
                    {
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintLibrary.GetBluePrint(scenefuniture.itemID);
                        var buildItem = Instantiate(bluePrint.buildPrefab, scenefuniture.position.ToVector3(), Quaternion.identity, itemParent);

                        if (buildItem.GetComponent<Box>())
                        {
                            buildItem.GetComponent<Box>().InitBox(scenefuniture.boxIndex);
                        }
                    }
                }
            }
        }
    }

}
