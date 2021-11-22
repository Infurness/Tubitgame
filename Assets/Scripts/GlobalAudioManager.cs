using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType {Music, Effect };
public class GlobalAudioManager : MonoBehaviour
{
    [SerializeField] private int numberOfAudioSourcesForPool;
    Queue<AudioSource> unusedAudioSources = new Queue<AudioSource>();
    float musicVolumeModifier = 1;
    float effectsVolumeModifier = 1;
    private void Awake ()
    {
        for(int i=0; i<numberOfAudioSourcesForPool;i++)
        {
            unusedAudioSources.Enqueue (gameObject.AddComponent<AudioSource> ());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMusicVolumeModifier (float value)
    {
        effectsVolumeModifier = value;
    }
    public void SetEffectsVolumeModifier (float value)
    {
        effectsVolumeModifier = value;
    }
    //Pooling system: Get aud out of the available queue, wait for it to end and return it to the Queue
    public void PlaySound(AudioClip clip, AudioType audioType, bool loop = false, float volume=1f)
    {
        AudioSource aud = GetAud ();
        if (aud == null)
        {
            Debug.LogError ($"Not Audiosources available for playing this clip: {clip.name}");
            return;
        }
        aud.clip = clip;
        float multiplier = audioType == AudioType.Music ? musicVolumeModifier * musicVolumeModifier : effectsVolumeModifier * effectsVolumeModifier; //Non linear volume, more acurracy to a good audio without using Logarithms
        aud.volume = volume * multiplier;
        aud.loop = loop;
        aud.Play ();

        StartCoroutine (WaitUntilAudFinishes (aud));
    }
    AudioSource GetAud ()
    {
        if (unusedAudioSources.Count > 0)
        {
            AudioSource takenAud = unusedAudioSources.Dequeue ();
            return takenAud;
        }
        else
        {
            return null;
        }
    }

    IEnumerator WaitUntilAudFinishes(AudioSource aud)
    {
        while (aud.isPlaying)
        {
            yield return null;
        }
        unusedAudioSources.Enqueue (aud);
    }
}
