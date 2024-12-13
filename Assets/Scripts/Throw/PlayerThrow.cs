using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow: MonoBehaviour
{
    public Transform holdPoint;
    public float throwForceBase = 5f;
    public float throwForceMax = 20f; 
    public float maxPickupMass = 5f;
    public float rotateSpeed = 100f; 

    private Rigidbody heldObject;
    private Collider heldObjectCollider;
    private float holdDuration = 0f;
    private bool isThrowing = false; 
    private bool isRotating = false; 

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                DropItem();
            }
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
            Rigidbody targetRb = hit.collider.GetComponent<Rigidbody>();

            if (targetRb != null && targetRb.mass <= maxPickupMass)
            {
                heldObject = targetRb;
                heldObjectCollider = heldObject.GetComponent<Collider>();

               
                heldObject.constraints = RigidbodyConstraints.FreezeRotation;
                heldObject.useGravity = false; 
                heldObject.velocity = Vector3.zero;
                heldObject.angularVelocity = Vector3.zero;

                
                if (heldObjectCollider != null)
                {
                    Physics.IgnoreCollision(heldObjectCollider, GetComponent<Collider>(), true);
                }

                
                heldObject.transform.position = holdPoint.position;
                heldObject.transform.rotation = holdPoint.rotation; 
                heldObject.transform.rotation = holdPoint.rotation; 
                heldObject.transform.parent = holdPoint; 
            }
        }
    }

    void DropItem()
    {
        heldObject.useGravity = true;
        heldObject.transform.parent = null;

        
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

        
        float force = Mathf.Lerp(throwForceBase, throwForceMax, chargeTime / 3f); 
        force = Mathf.Clamp(force, throwForceBase, throwForceMax);

       
        heldObject.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);

        heldObject = null;
    }

    void RotateItem()
    {
        heldObject.constraints = RigidbodyConstraints.None;
        heldObject.transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime, Space.Self); 
        heldObject.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self); 
        heldObject.transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.Self); 
    }

    void FollowPlayer()
    {
       
        heldObject.transform.position = holdPoint.position;
        if (!isRotating)
        {
            heldObject.transform.rotation = holdPoint.rotation;
        }
        //heldObject.transform.rotation = holdPoint.rotation;
    }
}
