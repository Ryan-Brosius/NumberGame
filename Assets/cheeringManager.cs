using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class cheeringManager : MonoBehaviour
{
    [SerializeField] private List<AudioResource> mainCheers;
    [SerializeField] private List<AudioResource> cheerAdditions;

    [SerializeField] private AudioSource mainCheerSource;
    [SerializeField] private AudioSource additionalCheerSource;

    private float additionDelay = 0.05f;
    private bool additionPlayed = false;
    private bool isPlaying = false;

    private static cheeringManager instance = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        additionDelay = 0.05f;
    }

    void Update()
    {
        if (!additionPlayed && isPlaying)
        {
            if (additionDelay > 0f){
                additionDelay -= Time.deltaTime;
            }
            else
            {
                var cheerAdditionSound = cheerAdditions[Random.Range(0, cheerAdditions.Count - 1)];
                additionalCheerSource.resource = cheerAdditionSound;
                additionalCheerSource.Play();
                additionPlayed = true;
            }
        }
    }
    
    public void PlayCheerSfx()
    {
        isPlaying = true;

        // choose main sound
        var rand = Random.value;
        var cheerSound = mainCheers[0];

        if (rand < 0.35f){
            cheerSound = mainCheers[0];
        }
        else if (rand < 0.7f)
        {
            cheerSound = mainCheers[1];
        }
        else if (rand < 0.85f)
        {
            // make the more obnoxious cheers rarer
            cheerSound = mainCheers[2];
        }
        else
        {
            cheerSound = mainCheers[3];
        }

        mainCheerSource.resource = cheerSound;
        mainCheerSource.Play();

        rand = Random.value;

        if (rand < 0.5f)
        {
            additionPlayed = true;
        }
    }
}
