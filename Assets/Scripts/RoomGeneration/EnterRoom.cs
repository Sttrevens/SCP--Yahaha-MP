using System.Collections;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    public float rotationSpeed = 100f;  
    [SerializeField] private float rotationAmount = -74f; 
    private bool isRotating = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && !isRotating)
        {
            StartCoroutine(RotateGradually());
        }
        
        if (Input.GetKeyDown(KeyCode.R) && isRotating)
        {
            GameObject.Find("CurrentPlayer").transform.position = GameObject.Find("SpawnPoint").transform.position;
        }
    }

    IEnumerator RotateGradually()
    {
        isRotating = true;
        float totalRotation = 0f;  
        float rotationStep = rotationSpeed * Time.deltaTime;  
        float direction = Mathf.Sign(rotationAmount);

        while (Mathf.Abs(totalRotation) < Mathf.Abs(rotationAmount))
        {
            float rotationThisFrame = Mathf.Min(rotationStep, Mathf.Abs(rotationAmount) - Mathf.Abs(totalRotation));
            transform.Rotate(Vector3.left, direction * rotationThisFrame);
            totalRotation += direction * rotationThisFrame;
            yield return null;  
        }

        transform.Rotate(Vector3.left, rotationAmount - totalRotation);
    }
}
