using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DestroyIt;

namespace LPSurvivalEngine
{
    public class WieldableAxe : Wieldable
    {  
    [Space]
    [Header("Wieldable Tool")]
    [Space]
    [Space]

    [Space]
    [Header("Hit Settings")]
    [Space]

    public float hitRate;
    public float hitDistance;
    
    [Space]
    [Header("Combat")] 
    [Space]

    public bool doesDealDamage;
    public int damage;

    [Space]
    [Header("Gathering")]
    [Space]

    public bool doesGatherresources;

    [Space]
    [Header("Assignments")]
    [Space]

    public Animator anim;

    public bool hitting;
    private Camera cam;


    private void Awake()
    {
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!hitting)
        {
            hitting = true;
            anim.SetTrigger("TwoHandAttack");
            Invoke("OnCanAttack", hitRate);
        }
    }

    void OnCanAttack()
    {
        
        OnHit();
        hitting = false;
    }

        public void OnHit()
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, hitDistance))
            {
                // 资源采集
                if (doesGatherresources && hit.collider.GetComponent<ResourceTree>())
                {
                    hit.collider.GetComponent<ResourceTree>().Gather(hit.point, hit.normal);
                }

                // 对实现了IDamagable接口的物体造成伤害
                if (doesDealDamage && hit.collider.GetComponent<Destructible>() != null)
                {
                    hit.collider.GetComponent<Destructible>().ApplyDamage(damage);
                }
            }
        }
    }


}