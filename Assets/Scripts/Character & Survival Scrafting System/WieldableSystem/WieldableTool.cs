using DestroyIt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LPSurvivalEngine
{
    [Serializable]
    public enum WieldableType
    {
        Tool,
        BluntMelee,
        SharpMelee,
        Ranged
    }

    public class WieldableTool : Wieldable
    {
        [Space]
        [Header("Wieldable Tool")]
        [Space]

        public bool isOneHanded;
        [Space]

    [Space]
    [Header("Hit Settings")]
    [Space]

    public float hitRate;
        public float hitCoolDownTime;
    public float hitDistance;
    
    [Space]
    [Header("Combat")] 
    [Space]
    
    public WieldableType wieldableType;
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
                if (isOneHanded)
                {
                    anim.SetTrigger("OneHandAttack");
                }
                else
                {
                    anim.SetTrigger("TwoHandAttack");
                }
                Invoke("OnCanAttack", hitRate);
                //PlayerController.instance.SetIsAttacking(true);
            }
        }

        void OnCanAttack()
        {
            OnHit();

            StartCoroutine(DelaySetAttackingFalse());
        }

        private IEnumerator DelaySetAttackingFalse()
        {
            yield return new WaitForSeconds(hitCoolDownTime);
            hitting = false;
        }

        public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hitDistance))
        {
            if (doesGatherresources && hit.collider.GetComponent<Resources>())
            {
                hit.collider.GetComponent<Resources>().Gather(hit.point, hit.normal);
            }

                if (doesDealDamage && hit.collider.GetComponent<Destructible>() != null)
                {
                    if (wieldableType == WieldableType.BluntMelee)
                    {
                        hit.collider.GetComponent<Destructible>().ApplyDamage(damage);
                    }
                    else
                    {
                        hit.collider.GetComponent<Destructible>().ApplyDamage(damage / 2);
                    }
                    DestructibleBarController.Instance.UpdateHealthBar(hit.collider.GetComponent<Destructible>().CurrentHitPoints, hit.collider.GetComponent<Destructible>().TotalHitPoints);
                }

            }
    }
    
}


}