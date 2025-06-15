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

        void Start()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }

        void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        }

        
        void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        }

        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
           var newItem =  Instantiate(itemPrefab,pos,Quaternion.identity,itemParent);
           newItem.itemID = ID;
        }
    }

}
