using HighlightPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        public TextMeshProUGUI hintObjectText;
        public TextMeshProUGUI hintInteractText;
        public TextMeshProUGUI hintLiftText;


        private GameObject currentInteractGameObject;
    private IInteractable currentInteractable;
    private Camera cam;

        public PlayerInput PlayerInput;
        private InputAction interactAction;

        public Image crosshairImage;
        public Sprite knifeIcon;
        private Sprite crosshairOriginalIcon;
        private void Start()
        {
            cam = Camera.main;

            if (PlayerInput != null) {
                interactAction = PlayerInput.actions.FindAction("Interact");
            }

            crosshairOriginalIcon = crosshairImage.sprite;

            hintObjectText.text = "";
            hintInteractText.text = "";
            hintLiftText.text = "";
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
                    // ����µĽ�������͵�ǰ���岻ͬ��ȡ����ǰ����ĸ���
                    if (hit.collider.gameObject != currentInteractGameObject)
                    {
                        if (currentInteractGameObject != null)
                        {
                            var previousHighlightEffect = currentInteractGameObject.GetComponent<HighlightEffect>();
                            if (previousHighlightEffect != null)
                            {
                                previousHighlightEffect.highlighted = false; // ȡ������
                            }
                        }

                        currentInteractGameObject = hit.collider.gameObject;
                        currentInteractable = hit.collider.GetComponent<IInteractable>();
                        Interaction();
                    }
                }
                else
                {
                    // ���û�������κ����壬ȡ����ǰ����ĸ���
                    if (currentInteractGameObject != null)
                    {
                        var previousHighlightEffect = currentInteractGameObject.GetComponent<HighlightEffect>();
                        if (previousHighlightEffect != null)
                        {
                            previousHighlightEffect.highlighted = false; // ȡ������
                        }
                    }

                    currentInteractGameObject = null;
                    currentInteractable = null;
                    interact.gameObject.SetActive(false);
                }

                if (Physics.Raycast(ray, out hit, maxCheckDistance))
                {
                    if (hit.collider.gameObject.GetComponent<ChoppedItems>() != null)
                    {
                        if (hit.collider.gameObject.GetComponent<ChoppedItems>().canBeChopped)
                        {
                            crosshairImage.sprite = knifeIcon;
                        }
                    }
                }
                else { crosshairImage.sprite = crosshairOriginalIcon; }

                if (Physics.Raycast(ray, out hit, maxCheckDistance))
                {
                    if (hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<Rigidbody>() != null)
                    {
                        hintLiftText.text = string.Format("Hold {0} to lift", "E");
                    }
                    else
                    {
                        hintLiftText.text = "";
                    }
                }
                else
                {
                    hintLiftText.text = "";
                }


            }

            if (currentInteractGameObject != null)
            {
                if (currentInteractGameObject.GetComponent<ItemObject>() != null)
                {
                    hintObjectText.text = currentInteractGameObject.GetComponent<ItemObject>().name;
                    hintInteractText.text = string.Format("Use {0} to pick up", "E");
                }
                else
                {
                    hintInteractText.text = "";
                }
            }
            else
            {
                hintInteractText.text = "";
                hintObjectText.text = "";
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

                var vendingMachineController = currentInteractGameObject.GetComponent<VendingMachineController>();
                if (vendingMachineController != null)
                {
                    vendingMachineController.SetPlayer(this.gameObject);
                }

                var sleepingBag = currentInteractGameObject.GetComponent<BedLikeController>();
                if (sleepingBag != null)
                {
                    sleepingBag.SetPlayer(this.gameObject);
                }

                var craftBench = currentInteractGameObject.GetComponent<CraftBench>();
                if (craftBench != null)
                {
                    craftBench.SetPlayer(this.GetComponent<PlayerController>());
                }

                var watchController = currentInteractGameObject.gameObject.GetComponent<WatchController>();
                if (watchController != null)
                {  watchController.SetPlayer(this.gameObject);}

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
    //string GetObjectName();
}