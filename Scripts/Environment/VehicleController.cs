using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VehicleController : MonoBehaviour
{
    private AudioSource source;
    private Animator animator;
    private Sprite sprite;

    private float alertDuration;
    private float alertStartTime;

    public void Awake()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>().sprite;
        alertDuration = source.clip.length;
        source.Stop();
        animator.enabled = false;
    }

    private void Update()
    {
        if (Time.time - alertStartTime >= alertDuration)
        {
            animator.enabled = false;
            animator.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.ShakeCamera();
    }

    public void DamagedEffect(MessageAttackEffect message)
    {
        Vector3 contactProximatePosition = (message.target + message.origin) / 2f;
        GameManager.instance.effectDisplayController.PlayBlastEffect(contactProximatePosition);
        if (source != null && !source.isPlaying)
        {
            source.Play();
            // animator.Play("LimoWhiteAlert", -1, alertDuration) ;
            alertStartTime = Time.time;
            animator.enabled = true;
        }
    }
}
