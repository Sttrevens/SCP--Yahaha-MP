using System.Collections;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;  // ÿתٶȣ/룩
    public float rotationAmount = -80.0f;  // ĿתǶ

    private bool isRotating = false;  // Ƿת
    private Quaternion initialRotation;  //ʼת
    private Quaternion targetRotation;   // Ŀת

    void Start()
    {
        Debug.Log("[EnterRoom] Initializing rotation settings");
        //ʼת״̬
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(rotationAmount, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        Debug.Log($"[EnterRoom] Initial rotation: {initialRotation.eulerAngles}, Target rotation: {targetRotation.eulerAngles}");
    }

    /// <summary>
    /// ƽתĿλ
    /// </summary>
    public void StartRotation()
    {
        Debug.Log("[EnterRoom] StartRotation called");
        if (!isRotating)
        {
            Debug.Log("[EnterRoom] Starting rotation to target");
            StartCoroutine(RotateToTarget(targetRotation));
        }
        else
        {
            Debug.Log("[EnterRoom] Rotation already in progress, ignoring request");
        }
    }

    /// <summary>
    /// ƽλʼλ
    /// </summary>
    public void ResetRotation()
    {
        Debug.Log("[EnterRoom] ResetRotation called");
        if (!isRotating)
        {
            Debug.Log("[EnterRoom] Starting rotation to initial position");
            StartCoroutine(RotateToTarget(initialRotation));
        }
        else
        {
            Debug.Log("[EnterRoom] Rotation already in progress, ignoring request");
        }
    }

    /// <summary>
    /// ƽתָĿ
    /// </summary>
    /// <param name="target">Ŀת</param>
    IEnumerator RotateToTarget(Quaternion target)
    {
        Debug.Log($"[EnterRoom] Starting RotateToTarget coroutine. Target rotation: {target.eulerAngles}");
        isRotating = true;

        // ֵ֡ת
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            Debug.Log($"[EnterRoom] Current rotation: {transform.rotation.eulerAngles}, Remaining angle: {Quaternion.Angle(transform.rotation, target)}");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
            yield return null;  // ȴһ֡
        }

        // ȷתȫ
        transform.rotation = target;
        Debug.Log($"[EnterRoom] Rotation complete. Final rotation: {transform.rotation.eulerAngles}");

        isRotating = false; //ת
        Debug.Log("[EnterRoom] Rotation finished");
    }
}
