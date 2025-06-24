using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Farm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;

        
        void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        
        void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        }

        private void OnAfterSceneLoadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }

        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
           var newItem =  Instantiate(itemPrefab,pos,Quaternion.identity,itemParent);
           newItem.itemID = ID;
        }
    }

}
