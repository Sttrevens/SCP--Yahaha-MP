using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace LPSurvivalEngine
{
    public class DamageZone : NetworkBehaviour
    {
    [Space]
    [Header("Damage Zone")]
    [Space]

    public int damage = 10;
    public float damageRate = 1;

    private List<IDamagable> thingsToDamage = new List<IDamagable>();
    
    public override void Spawned()
    {
        StartCoroutine(DealDamage());
    }

    IEnumerator DealDamage()
    {
        while(true)
        {
            for (int i = 0; i < thingsToDamage.Count; i++)
            {
                thingsToDamage[i].Rpc_TakePhysicDamage(damage);
            }
            yield return new WaitForSeconds(damageRate);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Add(other.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Remove(other.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Add(collision.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Remove(other.gameObject.GetComponent<IDamagable>());
        }
    }
    }
}

