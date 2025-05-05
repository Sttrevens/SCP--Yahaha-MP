using System.Collections;
using System.Collections.Generic;
using LPSurvivalEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BobojianZhuBaoTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    public TextMeshProUGUI noSignalText;
    public TextMeshProUGUI settledStatusText;
    public PlayerData playerData;
    [SerializeField] private GameObject blackScreen;
    private RenderTexture myRenderTexture;
    private bool playerNameLocked = false; // 标记玩家名称是否已锁定

    private void Start()
    {
        // 取当前对象身上的 RawImage 组件的 RenderTexture
        RawImage rawImg = GetComponent<RawImage>();
        if (rawImg != null)
        {
            myRenderTexture = rawImg.texture as RenderTexture;
        }
    }

    private void FixedUpdate()
    {
        if (myRenderTexture == null)
        {
            Debug.LogWarning("myRenderTexture is null, skipping FixedUpdate.");
            return;
        }

        GameObject[] liveCameraObjects = GameObject.FindGameObjectsWithTag("LiveCamera");
        if (liveCameraObjects == null || liveCameraObjects.Length == 0)
        {
            if (noSignalText != null && !ControlSticksController.Instance.isSettling)
            {
                noSignalText.gameObject.SetActive(true);
            }
            if (blackScreen != null) blackScreen.SetActive(true);
            Debug.LogWarning("No GameObjects with tag 'LiveCamera' found.");
            return;
        }
        
        foreach (GameObject liveCamObj in liveCameraObjects)
        {
            if (liveCamObj == null)
            {
                if (noSignalText != null && !ControlSticksController.Instance.isSettling)
                {
                    noSignalText.gameObject.SetActive(true);
                }
                if (blackScreen != null) blackScreen.SetActive(true);
                continue;
            }

            Transform photoCamTransform = liveCamObj.transform.Find("PhotoCamera");
            if (photoCamTransform == null)
            {
                if (noSignalText != null && !ControlSticksController.Instance.isSettling)
                {
                    noSignalText.gameObject.SetActive(true);
                }
                if (blackScreen != null) blackScreen.SetActive(true);
                Debug.LogWarning("PhotoCamera not found in " + liveCamObj.name);
                continue;
            }

            Camera cam = photoCamTransform.GetComponent<Camera>();
            if (cam == null)
            {
                if (noSignalText != null && !ControlSticksController.Instance.isSettling)
                {
                    noSignalText.gameObject.SetActive(true);
                }
                if (blackScreen != null) blackScreen.SetActive(true);
                Debug.LogWarning("Camera component not found on PhotoCamera in " + liveCamObj.name);
                continue;
            }
            
            if (cam.targetTexture != myRenderTexture)
            {
                //if (noSignalText != null) noSignalText.gameObject.SetActive(true);
                Debug.Log("Camera on " + liveCamObj.name + " does not use the expected RenderTexture.");
                continue;
            }
            
            // Find the top parent of the live camera object
            Transform topParent = liveCamObj.transform;
            while (topParent.parent != null)
            {
                topParent = topParent.parent;
            }

            playerData = topParent.GetComponent<PlayerData>();
            if (playerData == null)
            {
                Debug.LogWarning("PlayerData component not found on top parent: " + topParent.name);
                continue;
            }
            
            if (playerNameText == null)
            {
                Debug.LogWarning("playerNameText is not assigned on " + gameObject.name);
                continue;
            }
            
            // 只有在未锁定状态下才更新玩家名称
            if (!playerNameLocked)
            {
                playerNameText.text = "monitoring: " + playerData.PlayerName;
                playerNameLocked = true; // 一旦设置了有效的玩家名称，就锁定它
            }
            
            if (noSignalText != null) noSignalText.gameObject.SetActive(false);
            if (blackScreen != null) blackScreen.SetActive(false);
        }
    }

    // 提供一个公共方法来解锁玩家名称显示
    public void UnlockPlayerNameDisplay()
    {
        playerNameLocked = false;
    }
}