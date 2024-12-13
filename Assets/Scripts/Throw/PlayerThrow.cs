using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    public Transform holdPoint;
    public float throwForceBase = 5f;
    public float throwForceMax = 20f;
    public float maxPickupMass = 5f;
    public float rotateSpeed = 100f;
    public float pickupSmoothDuration = 0.15f; // 平滑拿取的持续时间

    private Rigidbody heldObject;
    private Collider heldObjectCollider;
    private float holdDuration = 0f;
    private bool isThrowing = false;
    private bool isRotating = false;

    private bool isInteracting = false;

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            holdDuration += Time.deltaTime;
            if (holdDuration >= 1f && !isInteracting)
            {
                isInteracting = true;
                if (heldObject == null)
                {
                    TryPickup();
                }
                else
                {
                    DropItem();
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            holdDuration = 0f;
            isInteracting = false;
        }

        if (Input.GetMouseButton(0) && heldObject != null)
        {
            isThrowing = true;
            holdDuration += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && heldObject != null)
        {
            if (holdDuration >= 1f)
            {
                ThrowItem(holdDuration);
            }
            else
            {
                DropItem();
            }

            holdDuration = 0f;
            isThrowing = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && heldObject != null)
        {
            isRotating = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            isRotating = false;
        }

        if (isRotating && heldObject != null)
        {
            RotateItem();
        }

        if (heldObject != null)
        {
            FollowPlayer();
        }
    }

    void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            Rigidbody targetRb = hit.collider?.GetComponent<Rigidbody>();
            if (targetRb != null && targetRb.mass <= maxPickupMass)
            {
                heldObject = targetRb;
                heldObjectCollider = heldObject.GetComponent<Collider>();
                heldObject.useGravity = false;
                heldObject.velocity = Vector3.zero;
                heldObject.angularVelocity = Vector3.zero;

                if (heldObjectCollider != null)
                {
                    Physics.IgnoreCollision(heldObjectCollider, GetComponent<Collider>(), true);
                }

                StartCoroutine(SmoothPickup());
            }
        }
        else
        {
            isInteracting = false;
        }
    }

    IEnumerator SmoothPickup()
    {
        Vector3 initialPosition = heldObject.transform.position;
        Quaternion initialRotation = heldObject.transform.rotation;

        float elapsed = 0f;
        while (elapsed < pickupSmoothDuration)
        {
            if (heldObject == null)
            {
                yield break; 
            }

            elapsed += Time.deltaTime;
            float t = elapsed / pickupSmoothDuration;

            heldObject.transform.position = Vector3.Lerp(initialPosition, holdPoint.position, t);
            Quaternion targetRotation = Quaternion.Euler(holdPoint.rotation.eulerAngles.x, holdPoint.rotation.eulerAngles.y, initialRotation.eulerAngles.z);
            heldObject.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

            yield return null;
        }

        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
            heldObject.transform.rotation = Quaternion.Euler(holdPoint.rotation.eulerAngles.x, holdPoint.rotation.eulerAngles.y, initialRotation.eulerAngles.z);
            heldObject.transform.parent = holdPoint;
            heldObject.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void DropItem()
    {
        heldObject.useGravity = true;
        heldObject.transform.parent = null;
        isInteracting = false;

        if (heldObjectCollider != null)
        {
            Physics.IgnoreCollision(heldObjectCollider, GetComponent<Collider>(), false);
        }

        heldObject.constraints = RigidbodyConstraints.None;

        heldObject = null;
    }

    void ThrowItem(float chargeTime)
    {
        heldObject.useGravity = true;
        heldObject.transform.parent = null;

        if (heldObjectCollider != null)
        {
            Physics.IgnoreCollision(heldObjectCollider, GetComponent<Collider>(), false);
        }

        heldObject.constraints = RigidbodyConstraints.None;

        float force = Mathf.Lerp(throwForceBase, throwForceMax, chargeTime / 5f);
        force = Mathf.Clamp(force, throwForceBase, throwForceMax);

        heldObject.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);

        heldObject = null;
        isInteracting = false;
    }

    void RotateItem()
    {
        heldObject.constraints = RigidbodyConstraints.None;
        heldObject.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.Self);
    }

    void FollowPlayer()
    {
        heldObject.transform.position = holdPoint.position;
        if (!isRotating)
        {
            heldObject.transform.rotation = Quaternion.Euler(holdPoint.rotation.eulerAngles.x, holdPoint.rotation.eulerAngles.y, heldObject.transform.rotation.eulerAngles.z);
        }
    }
}