using UnityEngine;
 
public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float Height = 0.7f;
    public float MouseSensitivity = 10f;
    
    public bool isCameraLocked = false;
 
    private float verticalRotation;
    private float horizontalRotation;
 
    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }
        
        transform.position = Target.position;
        transform.position += new Vector3(0,Height,0);

        if (isCameraLocked)
        {
            return;
        }
 
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
 
        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);
 
        horizontalRotation += mouseX * MouseSensitivity;
 
        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }
}