using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    public List<AudioResource> musicTrack;

    private static MusicManager instance = null;
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

    private void Start()
    {
        if (musicTrack.Count > 0){
            if (musicTrack[0] != null)
            {
                if (!audioSource.isPlaying)
                {
                    PlayMusic(musicTrack[0], 0f);
                }
            }
        }
    }

    public static void PlayMusic(AudioResource music, float fadeInTime = 0.1f)
    {
        Debug.Log("PlayMusic");
        instance.audioSource.resource = music;
        instance.audioSource.Play();
    }

    public static void FadeOut()
    {
        Debug.Log("Fade Out Music");
    }

    public static void StopMusic()
    {
        Debug.Log("Stop Music");
    }
}