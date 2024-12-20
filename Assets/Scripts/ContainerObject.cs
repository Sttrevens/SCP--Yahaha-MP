using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ContainerObject : MonoBehaviour, IInteractable
    {
        public ItemDatabase[] itemsInContainer;

        public string objectDisplayName;

        private ContainerInventory inventory;
        private bool isInitialized = false;

        private void Awake()
        {
            inventory = GetComponent<ContainerInventory>();
            
            // 如果没有挂载，则自动添加 ContainerInventory 组件
            if (inventory == null)
            {
                inventory = gameObject.AddComponent<ContainerInventory>();
            }
        }
        

        public string GetInteractText()
        {
            return string.Format("{0}", objectDisplayName);
        }

        public void OnInteract()
        {
            if (!isInitialized && itemsInContainer != null)
            {
                foreach (var item in itemsInContainer)
                {
                    inventory.AddItemToContainer(item);
                }
            }

            Inventory.instance.InteractWithContainer(inventory);
            GetInteractText();

            isInitialized = true;
        }


    }
}