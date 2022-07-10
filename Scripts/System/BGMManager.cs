using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{

    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float audioLength = audioSource.clip.length;
        float startPoint = Random.Range(0, audioLength);
        audioSource.time = startPoint;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
