using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion
{
    public class CoroutineRunner : MonoBehaviour
    {
        public static CoroutineRunner Instance {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            } }
        
        private static CoroutineRunner instance;

        public IEnumerable ExecuteCoroutine(IEnumerator routine, Action action)
        {
            yield return StartCoroutine(routine);
            action();
        }
    }
}
