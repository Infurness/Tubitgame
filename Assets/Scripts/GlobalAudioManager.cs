using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public enum AudioType {Music, Effect };
public class GlobalAudioManager : MonoBehaviour
{
    [Inject] SignalBus signalBus;
    [Inject] SoundsHolder soundsHolder;

    [SerializeField] private int numberOfAudioSourcesForPool;
    Queue<AudioSource> unusedAudioSources = new Queue<AudioSource>();
    private AudioSource musicAudioSource;
    float musicVolumeModifier = 1;
    float effectsVolumeModifier = 1;

    Dictionary<AudioType, List<AudioSource>> allAudios = new Dictionary<AudioType,List<AudioSource>>();
    private void Awake ()
    {
        allAudios.Add(AudioType.Effect, new List<AudioSource>());
        allAudios.Add(AudioType.Music,new List<AudioSource>());
        for (int i=0; i<numberOfAudioSourcesForPool;i++)
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            unusedAudioSources.Enqueue (temp);
            allAudios[AudioType.Effect].Add(temp);
        }
        AudioSource musicTemp = gameObject.AddComponent<AudioSource>();
        musicAudioSource = musicTemp;
        allAudios[AudioType.Music].Add(musicTemp);
    }
    // Start is called before the first frame update
    void Start()
    {
        signalBus.Subscribe<BuyItemSoundSignal>(PlayBuyItemSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMusicVolumeModifier (float value)
    {
        musicVolumeModifier = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        UpdateVolumes();
    }
    public void SetEffectsVolumeModifier (float value)
    {
        effectsVolumeModifier = value;
        PlayerPrefs.SetFloat("EffectsVolume", value);
        UpdateVolumes();
    }
    //Pooling system: Get aud out of the available queue, wait for it to end and return it to the Queue
    public void PlaySound(AudioClip clip, AudioType audioType, bool loop = false, bool unique = false)
    {
        if (unique)
            if (CheckIfClipIsPlaying(clip))
                return;

        AudioSource aud = GetAud (audioType);
        if (aud == null)
        {
            Debug.LogError ($"Not Audiosources available for playing this clip: {clip.name}");
            return;
        }
        aud.clip = clip;
        float multiplier = audioType == AudioType.Music ? musicVolumeModifier * musicVolumeModifier : effectsVolumeModifier * effectsVolumeModifier; //Non linear volume, more acurracy to a good audio without using Logarithms
        aud.volume = multiplier;
        aud.loop = loop;
        aud.Play ();
        if(!loop)
            StartCoroutine (WaitUntilAudFinishes (aud));
    }
    public void TerminateSound (AudioClip clip)
    {
        foreach(AudioType type in allAudios.Keys)
        {
            foreach(AudioSource aud in allAudios[type])
            {
                if (aud.isPlaying)
                {
                    if (aud.clip == clip)
                        aud.Stop();
                }
            }
        }
    }
    bool CheckIfClipIsPlaying(AudioClip clip)
    {
        foreach (AudioType type in allAudios.Keys)
        {
            foreach (AudioSource aud in allAudios[type])
            {
                if (aud.isPlaying)
                {
                    if (aud.clip == clip)
                        return true;
                }
            }
        }
        return false;
    }
    void UpdateVolumes()
    {
        foreach (AudioType type in allAudios.Keys)
        {
            foreach (AudioSource aud in allAudios[type])
            {
                float multiplier = type == AudioType.Music ? musicVolumeModifier * musicVolumeModifier : effectsVolumeModifier * effectsVolumeModifier; //Non linear volume, more acurracy to a good audio without using Logarithms
                aud.volume = multiplier;
            }
        }
    }
    AudioSource GetAud (AudioType audioType)
    {
        if(audioType == AudioType.Effect)
        {
            if (unusedAudioSources.Count > 0)
            {
                AudioSource takenAud = unusedAudioSources.Dequeue();
                return takenAud;
            }
        }
        else if(audioType == AudioType.Music)
        {
            return musicAudioSource;
        }

        return null;
    }

    IEnumerator WaitUntilAudFinishes(AudioSource aud)
    {
        while (aud.isPlaying)
        {
            yield return null;
        }
        unusedAudioSources.Enqueue (aud);
    }

    void PlayBuyItemSound()
    {
        PlaySound(soundsHolder.buyItem, AudioType.Effect);
    }
}
