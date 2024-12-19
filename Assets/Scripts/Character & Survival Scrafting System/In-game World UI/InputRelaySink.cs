using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputRelaySink : MonoBehaviour
{
    [SerializeField] RectTransform CanvasTransform;

    GraphicRaycaster Raycaster;
    List<GameObject> DragTargets = new List<GameObject>();
    GameObject lastHoveredObject = null; // Keep track of the last hovered object.

    void Start()
    {
        Raycaster = GetComponent<GraphicRaycaster>();
    }

    public void OnCursorInput(Vector2 normalisedPosition)
    {
        // Calculate the position in canvas space.
        Vector3 mousePosition = new Vector3(CanvasTransform.sizeDelta.x * normalisedPosition.x,
                                            CanvasTransform.sizeDelta.y * normalisedPosition.y,
                                            0f);

        // Construct our pointer event.
        PointerEventData mouseEvent = new PointerEventData(EventSystem.current);
        mouseEvent.position = mousePosition;

        // Perform a raycast using the graphics raycaster.
        List<RaycastResult> results = new List<RaycastResult>();
        Raycaster.Raycast(mouseEvent, results);

        bool sendMouseDown = Input.GetMouseButtonDown(0);
        bool sendMouseUp = Input.GetMouseButtonUp(0);
        bool isMouseDown = Input.GetMouseButton(0);

        // Handle end drag events.
        if (sendMouseUp)
        {
            foreach (var target in DragTargets)
            {
                if (ExecuteEvents.Execute(target, mouseEvent, ExecuteEvents.endDragHandler))
                    break;
            }
            DragTargets.Clear();
        }

        // Keep track of the hovered object for pointer enter/exit events.
        GameObject currentHoveredObject = results.Count > 0 ? results[0].gameObject : null;

        if (currentHoveredObject != lastHoveredObject)
        {
            // Trigger pointer exit on the last hovered object.
            if (lastHoveredObject != null)
            {
                ExecuteEvents.Execute(lastHoveredObject, mouseEvent, ExecuteEvents.pointerExitHandler);
            }

            // Trigger pointer enter on the new hovered object.
            if (currentHoveredObject != null)
            {
                ExecuteEvents.Execute(currentHoveredObject, mouseEvent, ExecuteEvents.pointerEnterHandler);
            }

            // Update the last hovered object.
            lastHoveredObject = currentHoveredObject;
        }

        // Process raycast results.
        foreach (var result in results)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;
            eventData.pointerCurrentRaycast = eventData.pointerPressRaycast = result;

            if (isMouseDown)
                eventData.button = PointerEventData.InputButton.Left;

            var slider = result.gameObject.GetComponentInParent<UnityEngine.UI.Slider>();

            if (sendMouseDown)
            {
                if (ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.beginDragHandler))
                    DragTargets.Add(result.gameObject);

                if (slider != null)
                {
                    slider.OnInitializePotentialDrag(eventData);

                    if (!DragTargets.Contains(result.gameObject))
                        DragTargets.Add(result.gameObject);
                }
            }
            else if (DragTargets.Contains(result.gameObject))
            {
                eventData.dragging = true;
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.dragHandler);

                if (slider != null)
                {
                    slider.OnDrag(eventData);
                }
            }

            if (sendMouseDown)
            {
                if (ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerDownHandler))
                    break;
            }
            else if (sendMouseUp)
            {
                bool didRun = ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerUpHandler);
                didRun |= ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerClickHandler);

                if (didRun)
                    break;
            }
        }
    }
}
