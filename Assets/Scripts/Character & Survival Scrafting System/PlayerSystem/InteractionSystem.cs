using HighlightPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LPSurvivalEngine
{
    public class InteractionSystem : MonoBehaviour
    {
    [Space]
    [Header("Interaction System")]
    [Space]
    [Space]

    [Space]
    [Header("Settings")]
    [Space]

    public float checkRate = 0.05f;
    private float lastCheckTime;
    public float maxCheckDistance;

    [Space]
    [Header("Assignments")]
    [Space]

    public LayerMask layerMask;
    public GameObject interact;
    public TextMeshProUGUI interactText;


    private GameObject currentInteractGameObject;
    private IInteractable currentInteractable;
    private Camera cam;


    private void Start()
    {
        cam = Camera.main;
    }

        private void Update()
        {
            if (Time.time - lastCheckTime > checkRate)
            {
                lastCheckTime = Time.time;

                Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
                {
                    // 如果新的交互物体和当前物体不同，取消当前物体的高亮
                    if (hit.collider.gameObject != currentInteractGameObject)
                    {
                        if (currentInteractGameObject != null)
                        {
                            var previousHighlightEffect = currentInteractGameObject.GetComponent<HighlightEffect>();
                            if (previousHighlightEffect != null)
                            {
                                previousHighlightEffect.highlighted = false; // 取消高亮
                            }
                        }

                        currentInteractGameObject = hit.collider.gameObject;
                        currentInteractable = hit.collider.GetComponent<IInteractable>();
                        Interaction();
                    }
                }
                else
                {
                    // 如果没有命中任何物体，取消当前物体的高亮
                    if (currentInteractGameObject != null)
                    {
                        var previousHighlightEffect = currentInteractGameObject.GetComponent<HighlightEffect>();
                        if (previousHighlightEffect != null)
                        {
                            previousHighlightEffect.highlighted = false; // 取消高亮
                        }
                    }

                    currentInteractGameObject = null;
                    currentInteractable = null;
                    interact.gameObject.SetActive(false);
                }
            }
        }


        void Interaction()
        {
            if (currentInteractable == null)
            {
                Debug.LogWarning("No interactable object detected.");
                return;
            }

            interact.gameObject.SetActive(true);
            interactText.text = string.Format("{0}", currentInteractable.GetInteractText());
            Debug.Log("Interaction text updated: " + currentInteractable.GetInteractText());

            var highlightEffect = currentInteractGameObject.GetComponent<HighlightEffect>();
            if (highlightEffect != null)
            {
                highlightEffect.highlighted = true;
            }
            else
            {
                Debug.LogWarning("No HighlightEffect component found on object: " + currentInteractGameObject.name);
            }
        }



        public void OnInteractInput(InputAction.CallbackContext context)
        {
            Debug.Log("Current interactable: " + currentInteractable);
            if (context.phase == InputActionPhase.Canceled && currentInteractable != null)
            {
                var cookingSystem = currentInteractGameObject.GetComponent<CookingSystem>();
                if (cookingSystem != null)
                {
                    cookingSystem.SetPlayer(this.gameObject);
                }

                currentInteractable.OnInteract();

                currentInteractGameObject = null;
                currentInteractable = null;
                interact.gameObject.SetActive(false);
            }
        }
    }

}

    public interface IInteractable
{
    string GetInteractText();   
    void OnInteract();
}