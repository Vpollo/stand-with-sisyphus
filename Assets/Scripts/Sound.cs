using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;

    [Range(0, 1)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop;
    public bool spacial;

    [HideInInspector] 
    public AudioSource audioSource;
}
