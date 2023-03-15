using System;
using Unity.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager S;

    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.spatialBlend = sound.spacial ? 1f : 0f;
        }

        if (!S)
        {
            S = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Play("whitenoise");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"{name} audio requested is not present in audio manager");
            return;
        }
        s.audioSource.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"{name} audio requested is not present in audio manager");
            return;
        }
        
        s.audioSource.Stop();
    }

    public void SetVolume(string name, float volume)
    {
        if (volume is > 1f or < 0f)
        {
            Debug.LogWarning($"trying to apply volume {volume} to sound {name}, which is out of range");
            return;
        }
        
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"{name} audio requested is not present in audio manager");
            return;
        }

        s.audioSource.volume = volume;
    }
}
