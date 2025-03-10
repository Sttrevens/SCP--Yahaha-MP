using UnityEngine;

public class GameFuckingManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
    }
}