using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    public class DamageZone : MonoBehaviour
    {
    [Space]
    [Header("Damage Zone")]
    [Space]

    public int damage = 10;
    public float damageRate = 1;

    private List<IDamagable> thingsToDamage = new List<IDamagable>();
    
    private void Start()
    {
        StartCoroutine(DealDamage());
    }

    IEnumerator DealDamage()
    {
        while(true)
        {
            for (int i = 0; i < thingsToDamage.Count; i++)
            {
                thingsToDamage[i].TakePhysicDamage(damage);
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
    }
}

