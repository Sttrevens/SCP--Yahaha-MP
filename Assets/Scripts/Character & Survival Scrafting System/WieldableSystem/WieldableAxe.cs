using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DestroyIt;
using Fusion;

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
        public float hitCoolDownTime;
        public float hitDistance;

        [Space]
        [Header("Combat")]
        [Space]

        public WieldableType wieldableType = WieldableType.BluntMelee;
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
            NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
            foreach (NetworkObject networkObject in networkObjects)
            {
                if (networkObject.HasStateAuthority)
                {
                    // 如果该NetworkObject具有输入权限，则认为是当前操作的玩家对象
                    GameObject currentPlayerObject = networkObject.gameObject;
                    anim = currentPlayerObject.GetComponent<Animator>();
                    Debug.Log("当前操作的玩家对象是：" + currentPlayerObject.name);
                    break;
                }
            }
            cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!hitting)
        {
            hitting = true;
            anim.SetTrigger("TwoHandAttack");
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
                if (doesGatherresources && hit.collider.GetComponent<ResourceTree>())
                {
                    hit.collider.GetComponent<ResourceTree>().Gather(hit.point, hit.normal);
                }

                if (doesDealDamage && hit.collider.GetComponent<Destructible>() != null)
                {
                    hit.collider.GetComponent<Destructible>().ApplyDamage(damage);
                    DestructibleBarController.Instance.UpdateHealthBar(hit.collider.GetComponent<Destructible>().CurrentHitPoints, hit.collider.GetComponent<Destructible>().TotalHitPoints);
                }

                if (doesDealDamage && hit.collider.GetComponent<ChoppedItems>() != null)
                {
                    if (wieldableType == WieldableType.SharpMelee)
                    {
                        hit.collider.GetComponent<ChoppedItems>().BeingChopped(damage);
                    }
                }

                if (doesDealDamage && hit.collider.GetComponent<EnemyAI>() != null)
                {
                    hit.collider.GetComponent<EnemyAI>().TakeDamage(damage);
                }

                if (doesDealDamage && hit.collider.GetComponent<hitEffect>() != null)
                {
                    hit.collider.GetComponent<hitEffect>().Hit(damage, hit.point, hit.normal);
                }
            }
            }
        }
    }