using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint; 
    public float throwForce = 10f; 
    public float maxPickupMass = 5f; 

    private Rigidbody heldObject;

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

        if (Input.GetMouseButtonDown(0) && heldObject != null) 
        {
            ThrowItem();
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
                heldObject.useGravity = false; 
                heldObject.velocity = Vector3.zero; 
                heldObject.angularVelocity = Vector3.zero; 
                heldObject.transform.position = holdPoint.position; 
                heldObject.transform.parent = holdPoint; 
            }
        }
    }

    void DropItem()
    {
        heldObject.useGravity = true;
        heldObject.transform.parent = null; 
        heldObject = null; 
    }

    void ThrowItem()
    {
        heldObject.useGravity = true;
        heldObject.transform.parent = null; 

       
        heldObject.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
    }
}
