using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundItem
{
    public string name;

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.8f;
    [Range(0.1f, 3f)]
    public float pitch = 2f;

    [HideInInspector]
    public AudioSource source;

}
