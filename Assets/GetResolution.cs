using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetResolution : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = Screen.width + "x" + Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
