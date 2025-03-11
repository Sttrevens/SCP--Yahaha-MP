using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputRelaySink : MonoBehaviour
{
    [SerializeField] RectTransform CanvasTransform;

    private GraphicRaycaster Raycaster;
    private List<GameObject> DragTargets = new List<GameObject>();
    private GameObject lastHoveredObject = null; 

    public PlayerInput playerInput;
    private InputAction action;

    // 用来记录上一帧动作是按下还是没按下
    private bool wasDown = false; 

    void Start()
    {
        Raycaster = GetComponent<GraphicRaycaster>();

        if (playerInput != null)
        {
            // 在 PlayerInput actions 中找名为 "Action" 的 InputAction
            action = playerInput.actions.FindAction("Action");
            if (action != null)
            {
                // 启用该 Action（如果尚未启用）
                action.Enable();
            }
        }
        else
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }
    }

    public void OnCursorInput(Vector2 normalisedPosition)
    {
        // 1. 先基于 Action 计算“是否按下”“刚按下”和“刚抬起”
        float inputVal = action != null ? action.ReadValue<float>() : 0f;
        // 大于 0.5f 算作“按下”
        bool isMouseDown = (inputVal > 0.5f);
        bool sendMouseDown = isMouseDown && !wasDown;
        bool sendMouseUp   = !isMouseDown && wasDown;

        // 更新 wasDown，为下次进入时做对比
        wasDown = isMouseDown;

        // 2. 计算鼠标（或光标）在 Canvas 内的坐标
        Vector3 mousePosition = new Vector3(
            CanvasTransform.sizeDelta.x * normalisedPosition.x,
            CanvasTransform.sizeDelta.y * normalisedPosition.y,
            0f
        );

        // 3. 构造 PointerEventData，用于 UI Raycast
        PointerEventData mouseEvent = new PointerEventData(EventSystem.current);
        mouseEvent.position = mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        Raycaster.Raycast(mouseEvent, results);

        // 4. 若松开，先处理“结束拖拽”
        if (sendMouseUp)
        {
            foreach (var target in DragTargets)
            {
                if (ExecuteEvents.Execute(target, mouseEvent, ExecuteEvents.endDragHandler))
                    break;
            }
            DragTargets.Clear();
        }

        // 5. 处理指针 enter / exit
        GameObject currentHoveredObject = results.Count > 0 ? results[0].gameObject : null;
        if (currentHoveredObject != lastHoveredObject)
        {
            // 之前悬停的物体，执行 pointerExit
            if (lastHoveredObject != null)
            {
                ExecuteEvents.Execute(lastHoveredObject, mouseEvent, ExecuteEvents.pointerExitHandler);
            }
            // 新悬停物体，执行 pointerEnter
            if (currentHoveredObject != null)
            {
                ExecuteEvents.Execute(currentHoveredObject, mouseEvent, ExecuteEvents.pointerEnterHandler);
            }
            // 更新记录
            lastHoveredObject = currentHoveredObject;
        }

        // 6. 遍历所有检测到的 UI 元素，发事件
        foreach (var result in results)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;
            eventData.pointerCurrentRaycast = eventData.pointerPressRaycast = result;

            if (isMouseDown)
                eventData.button = PointerEventData.InputButton.Left;

            var slider = result.gameObject.GetComponentInParent<UnityEngine.UI.Slider>();

            // 如果刚按下，处理 beginDrag
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
            // 如果处于拖拽
            else if (DragTargets.Contains(result.gameObject))
            {
                eventData.dragging = true;
                ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.dragHandler);

                if (slider != null)
                {
                    slider.OnDrag(eventData);
                }
            }

            // 处理 pointerDown / pointerUp / pointerClick
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