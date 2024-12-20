using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private bool isOpen = false;

    [SerializeField] private Animation anim;
    public bool willAutoClose = false;
    [SerializeField] private float autoCloseTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (anim != null)
            {
                anim = GetComponent<Animation>();
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerAnimatoin()
    {
        StartCoroutine(TriggeringAnimation());
    }

    public IEnumerator TriggeringAnimation()
    {
        if (isOpen != true)
        {
            // Wait for door animation to play
            yield return new WaitForSeconds(1f);
            anim[anim.clip.name].normalizedTime = 0;
            anim[anim.clip.name].speed = 1;
            anim.Play();
            isOpen = true;

            if (anim[anim.clip.name].speed > 0 && willAutoClose)
            {
                Invoke("Closing", autoCloseTime);
            }
        }
    }

    private void Closing()
    {
        if (isOpen)
        {
            anim[anim.clip.name].normalizedTime = 1;
            anim[anim.clip.name].speed = -1;
            anim.Play();

            isOpen = false;
        }
    }    
}
