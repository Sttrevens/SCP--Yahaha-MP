using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenuTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("[Game]PreDesignedLevel");
        }
    }
}
