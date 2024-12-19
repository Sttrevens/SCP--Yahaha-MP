using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class IngameUI : MonoBehaviour, IInteractable
    {

        public string GetInteractText()
        {
            return string.Format("{0}", "Interact");
        }

        public void OnInteract()
        {
            if (GetComponent<UnityEngine.UI.Button>())
            {
                UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
                button.onClick.Invoke();
            }
        }

    }
}