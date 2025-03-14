using UnityEngine;

public class GameFuckingManager : MonoBehaviour
{
    
    private static ILogger logger = Debug.unityLogger;
    private static string kTAG = "MyGameTag";
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;
        
        logger.logEnabled = Debug.isDebugBuild;

        logger.Log(kTAG, "This log will be displayed only in debug build");
    }
}