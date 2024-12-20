using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LPSurvivalEngine
{
    public class RespawnManager : MonoBehaviour
    {
        public string showCaseScene;
        public string demoScene;

        public void OnRespawnButtonShowcase()
        {
            SceneManager.LoadScene(showCaseScene);
        }

        public void OnRespawnButtonDemo()
        {
            SceneManager.LoadScene(demoScene);
        }
    }
}

