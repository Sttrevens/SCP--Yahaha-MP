using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class ResourceStone : MonoBehaviour
    {
    [Space]
    [Header("Recource Stone")]
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

        Destroy(Instantiate(Particles, hitpoint, quaternion.LookRotation(hitNormal, Vector3.up)),1.0f);
        
        if (capacity <= 0)
            Destroy(gameObject);
        
    }
    
}


}