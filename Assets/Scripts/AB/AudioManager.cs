using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(AudioManager).ToString());
                    instance = singletonObject.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    private AudioSource bgmSource;
    //private AudioSource sfxSource;
    public float bgmVolume = 1f; 
    public float sfxVolume = 1f; 

    [SerializeField] private AudioClip startingMenuSound;
    [SerializeField] private AudioClip startButtonSound;
    [SerializeField] private AudioClip elevatorShakeSound;
    [SerializeField] private AudioClip elevatorCloseSound;
    [SerializeField] private AudioClip planetEnvironmentSound;
    [SerializeField] private AudioClip spaceStationEnvironmentSound;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.playOnAwake = false;

        // sfxSource = gameObject.AddComponent<AudioSource>();
        // sfxSource.loop = false;
        // sfxSource.volume = sfxVolume;
        // sfxSource.playOnAwake = false;

        PlayBGM(startingMenuSound);
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmSource.clip != bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        bgmSource.volume = bgmVolume;
    }

    public void PlaySFX(GameObject SFXobject, AudioClip sfxClip, float volume = 1f)
    {
        AudioSource newSfxSource = SFXobject.GetComponent<AudioSource>();

        if (newSfxSource == null)
        {
            newSfxSource = SFXobject.AddComponent<AudioSource>();
            newSfxSource.loop = false;
            newSfxSource.playOnAwake = false;
        }

        newSfxSource.volume = sfxVolume * volume;
        newSfxSource.spatialBlend = 1;

        newSfxSource.PlayOneShot(sfxClip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void ToggleBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
        else
        {
            bgmSource.Play();
        }
    }

    public bool IsAudioPlaying()
    {
        return bgmSource.isPlaying;
    }
    
    public void ShiftBGM(AudioClip newBGMClip, float fadeDuration = 1f)
    {
        StartCoroutine(ShiftBGMCoroutine(newBGMClip, fadeDuration));
    }

    private IEnumerator ShiftBGMCoroutine(AudioClip newBGMClip, float fadeDuration)
    {
        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newBGMClip;
        bgmSource.Play();

        while (bgmSource.volume < bgmVolume)
        {
            bgmSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        bgmSource.volume = bgmVolume;
    }
    
    
    //usage
    public void PlayStartButtonSound()
    {
        PlaySFX(gameObject, startButtonSound);
    }

    public void PlayElevatorShakeSound(GameObject _object)
    {
        PlaySFX(_object, elevatorShakeSound);
    }

    public void PlayElevatorCloseSound(GameObject _object)
    {
        PlaySFX(_object, elevatorCloseSound);
    }

    
}
