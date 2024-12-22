using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class AnimationTrigger : MonoBehaviour
{
    public bool isOpen = false;

    [SerializeField] private Animation anim;
    public bool willAutoClose = false;
    public float autoCloseTime = 5f;

    public UnityEvent onDoorOpened;
    public UnityEvent onDoorClosed;

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
        Debug.Log("Hehe:" + isOpen);
        if (isOpen != true)
        {
            anim[anim.clip.name].normalizedTime = 0;
            anim[anim.clip.name].speed = 1;
            anim.Play();
            Debug.Log("Stone£º" + anim.clip.name);
            isOpen = true;
            onDoorOpened.Invoke();

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
            onDoorClosed.Invoke();
        }
    }    
}
