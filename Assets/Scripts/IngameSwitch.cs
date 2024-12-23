using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LPSurvivalEngine
{
    public class IngameSwitch : MonoBehaviour, IInteractable
    {
        public string interactText;

        public UnityEvent interactEvent;

        public string GetInteractText()
        {
            return string.Format("{0}", interactText);
        }

        public void OnInteract()
        {
            interactEvent.Invoke();
        }

    }
}