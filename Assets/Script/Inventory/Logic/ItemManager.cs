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

        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
        }


        void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }



        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneLoadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreatSceneItems();
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
    }

}
