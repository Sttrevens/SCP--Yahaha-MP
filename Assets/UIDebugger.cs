using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键按下
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1, // 默认的鼠标输入
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count > 0)
            {
                GameObject clickedObject = raycastResults[0].gameObject; 
                Debug.Log("Current UI by mouse: " + clickedObject.name);
            }
            else
            {
                Debug.Log("It's not UI element that your mouse is clicking.");
            }
        }
    }
}
