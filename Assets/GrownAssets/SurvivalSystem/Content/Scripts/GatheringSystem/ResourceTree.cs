using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace LPSurvivalEngine
{
    public class ResourceTree : MonoBehaviour
    {
    [Space]
    [Header("Tree")]
    [Space]
    [Space]

    [Space]
    [Header("Assignments")]
    [Space]

    public Rigidbody rig;
    public Transform cam;

    [Space]
    [Header("Items")]
    [Space]

    public ItemDatabase item;

    [Space]
    [Header("Amount")]
    [Space]

    public int capacity;
    public int quantityPerHit = 1;

    [Space]
    [Header("Effects")]
    [Space]

    public GameObject Particles;
    public AudioSource Hit;


    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void ChopTree(Vector3 impactDirection)
    {
        rig.isKinematic = false;
        rig.useGravity = true;

        rig.AddForce(impactDirection * 10f);
    }

    IEnumerator DestroyTree()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public void Gather(Vector3 hitpoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0)
                break;

            capacity -= 1;
            
            Inventory.instance.AddItem(item);
        }
        
        Hit.Play();

        Destroy(Instantiate(Particles, hitpoint, quaternion.LookRotation(hitNormal, Vector3.up)), 1.0f);
        
        if (capacity <= 0)
        {
            StartCoroutine(DestroyTree());

            RaycastHit hit;

            if(Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if(hit.transform.CompareTag("Tree"))
                {
                    ChopTree(cam.TransformDirection(Vector3.forward));
                }
            }
        }
             
    }
    
}


}