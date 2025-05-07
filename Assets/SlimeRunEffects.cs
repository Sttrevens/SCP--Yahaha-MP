using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeRunEffects : MonoBehaviour
{
    [SerializeField] private AudioClip bouncingSound;

    public void PlayBouncingSound()
    {
        if (bouncingSound != null)
        {
            AudioManager.instance.PlaySFX(gameObject, bouncingSound);
        }
    }
}
