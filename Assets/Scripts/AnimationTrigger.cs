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

    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;

    public UnityEvent onDoorOpened;
    public UnityEvent onDoorClosed;

    private AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (anim != null)
            {
                anim = GetComponent<Animation>();
            }

        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerAnimatoin()
    {
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

    public void PlayOpenDoorSound()
    {
        AudioSource.PlayOneShot(openDoorSound);
    }

    public void PlayCloseDoorSound()
    {
        AudioSource.PlayOneShot(closeDoorSound);
    }
}
