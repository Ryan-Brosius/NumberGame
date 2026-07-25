using System;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    private static SoundManager instance = null;
    private AudioSource audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (!instance)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
    }

    public static void PlaySound(AudioResource sound, AudioSource source = null, float volume = 1)
    {
        Debug.Log("PlaySound");
        instance.audioSource.resource = sound;
        instance.audioSource.volume = volume;
        instance.audioSource.Play();
    }
}