using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LPSurvivalEngine
{
    public class RespawnManager : MonoBehaviour
    {
        public void OnRespawnButtonShowcase()
        {
            SceneManager.LoadScene("Showcase");
        }

        public void OnRespawnButtonDemo()
        {
            SceneManager.LoadScene("Demo");
        }
    }
}

