using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LPSurvivalEngine;

public class UIDebugger : MonoBehaviour
{
    private bool isAlt;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �������������
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1, // Ĭ�ϵ��������
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

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (isAlt)
            {
                PlayerController.instance.ToggleCursor(false);
                isAlt = false;
            }
            else
            {
                PlayerController.instance.ToggleCursor(true);
                isAlt = true;
            }
        }
    }
}
