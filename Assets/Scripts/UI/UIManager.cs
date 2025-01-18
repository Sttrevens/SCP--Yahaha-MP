using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ExitButton(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false); 
        }
        else
        {
            Debug.LogWarning("ExitButton: The panel is null!");
        }
    }
}