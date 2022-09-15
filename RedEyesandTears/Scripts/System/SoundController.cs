using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundNames
{
    public const string FatherPunch = "FatherPunch";
}

public class SoundController : MonoBehaviour
{

    public SoundItem[] soundList;

    private void Awake()
    {
        foreach (SoundItem sound in soundList)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
        }
    }

    public void PlaySound(string soundName)
    {
        SoundItem sound = System.Array.Find(soundList, x => x.name == soundName);
        if (sound != null)
        {
            sound.source.Play();
        }
    }

    public void PlaySound(string soundName, Vector3 position)
    {
        /*
         position 采用屏幕坐标系
         */
        SoundItem sound = System.Array.Find(soundList, x => x.name == soundName);
        if (sound != null)
        {
            sound.source.transform.position = position;
            sound.source.Play();
        }
    }

    public void PlaySoundMultipleTimes(string soundName, int times, float gap = 0.1f)
    {
        SoundItem sound = System.Array.Find(soundList, x => x.name == soundName);
        if (sound != null)
        {
            StartCoroutine(PlayerSoundMultipleTimesEnumerator(sound, times, gap));
        }
    }

    public IEnumerator PlayerSoundMultipleTimesEnumerator(SoundItem sound, int times, float gap)
    {
        for (int i = 0; i < times; i++)
        {
            sound.source.Play();
            yield return new WaitForSeconds(gap);
        }
    }

}
