using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ContainerObject : MonoBehaviour, IInteractable
    {
        [Space]
        [Header("Container")]
        [Space]

        public ItemDatabase[] itemsInContainer;

        public string objectDisplayName;

        private ContainerInventory inventory;

        private void Start()
        {
            inventory = FindObjectOfType<ContainerInventory>(true);
        }

        public string GetInteractText()
        {
            return string.Format("{0}", objectDisplayName);
        }

        public void OnInteract()
        {
            if (itemsInContainer != null)
            {
                foreach (var item in itemsInContainer)
                {
                    inventory.AddItemToContainer(item);
                }
                Inventory.instance.InteractWithContainer();
                GetInteractText();
            }

        }


    }
}