using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class hitEffect : MonoBehaviour
{
    public GameObject lightHitParticles;
    public GameObject heavyHitParticles;
    public int lightDamageAmountThreshold;
    public AudioSource hitSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(int damageAmount, Vector3 hitPoint, Vector3 hitNormal)
    {
        if ((lightHitParticles != null) && (damageAmount > 0 && damageAmount <= lightDamageAmountThreshold))
        {
            Destroy(Instantiate(lightHitParticles, hitPoint, Unity.Mathematics.quaternion.LookRotation(hitNormal, Vector3.up)), 10.0f);
        }
        else if ((heavyHitParticles != null) && (damageAmount > lightDamageAmountThreshold))
        {
            Destroy(Instantiate(heavyHitParticles, hitPoint, Unity.Mathematics.quaternion.LookRotation(hitNormal, Vector3.up)), 10.0f);
        }

        if (hitSound != null)
        {
            hitSound.Play();
        }
    }
}
