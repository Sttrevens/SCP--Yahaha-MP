using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenPermissionDoor : MonoBehaviour
{
    public bool isPermitted = false;

    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private TextMeshProUGUI tmpText;

    private const string isPermittedText = "Permission Granted.";
    private const string isntPermittedText = "No Permission.";

    public AnimationTrigger triggerObject;
    // Start is called before the first frame update
    void Start()
    {
        tmpText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleButtonClick(string displayText)
    {
        HideAllButtons();

        if (isPermitted)
        {
            displayText = isPermittedText;
            triggerObject.TriggerAnimatoin();
        }
        else { displayText = isntPermittedText; }

        ShowText(displayText);
    }

    private void HideAllButtons()
    {
        buttonsParent.SetActive(false);
    }

    private void ShowText(string displayText)
    {
        tmpText.text = displayText; 
        tmpText.gameObject.SetActive(true);

        if (triggerObject.willAutoClose)
        {
            StartCoroutine(ResetUI());
        }
    }

    public IEnumerator ResetUI()
    {
        yield return new WaitForSeconds(triggerObject.autoCloseTime + 1);

        tmpText.gameObject.SetActive(false);
        buttonsParent.SetActive(true);
    }

    public void ChangePermission(bool permission)
    {
        isPermitted = permission;
    }
}
