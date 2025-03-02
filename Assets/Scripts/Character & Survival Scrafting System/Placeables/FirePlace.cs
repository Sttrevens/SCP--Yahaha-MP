using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace LPSurvivalEngine
{
    public class FirePlace : Buildings, IInteractable
    {
    [Space]
    [Header("Fireplace")]
    [Space]

    [Space]
    [Header("Assignments")]
    [Space]

    public GameObject particle;

    [Space]
    [Header("Settings")]
    [Space]

    public bool canDamage;
    public int damage;
    public float damageRate;
    
    private bool isBurning;
    private List<IDamagable> thingsToDoDamage = new List<IDamagable>();
    private Animator anim;
    private InputManager inputManager;

    private void Awake() 
    {
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        inputManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>();
    }

    private void Start()
    {
        StartCoroutine(DealDamage());
    }

    IEnumerator DealDamage()
    {
        while(true)
        {
            if(isBurning && canDamage)
            {
                for (int x = 0; x < thingsToDoDamage.Count; x++)
                    thingsToDoDamage[x].Rpc_TakePhysicDamage(damage);  
            }

            yield return new WaitForSeconds(damageRate);
        }
    }

    public string GetInteractText()
    {
        return isBurning ? "Off" : "On";
    }

    public void OnInteract()
    {
        anim.SetTrigger("Pickup");

        isBurning = !isBurning;

        StartCoroutine(StartFire());
    }

    IEnumerator StartFire()
    {
        yield return new WaitForSeconds(0.5f);

        particle.SetActive(isBurning);
    }

    private void Update()
    {
        if(isBurning)
        {
            float x = Mathf.PerlinNoise(Time.time * 3.0f, 0.0f) / 5.0f;
            float z = Mathf.PerlinNoise(0.0f, Time.time * 3.0f) / 5.0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDoDamage.Add(collision.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDoDamage.Remove(collision.gameObject.GetComponent<IDamagable>());
        }
    }
}


}