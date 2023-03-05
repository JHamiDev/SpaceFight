using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private static bool created = false;

    public static AudioManager instance;

    public AudioMixerGroup audioMixerGroup;

    void Awake()
    {
        if (!created)
        {
            //make first instance persist
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            instance = this;
            created = true;
        } else
        {
            //destroy duplicate instances from scene reloads
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.outputAudioMixerGroup = audioMixerGroup;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
}
