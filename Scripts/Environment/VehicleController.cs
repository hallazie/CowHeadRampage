using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{

    public List<Sprite> spriteList;
    public Sprite sprite;

    private AudioSource source;
    private Animator animator;

    private float alertDuration;
    private float alertStartTime;

    public void Awake()
    {
        if (sprite == null)
        {
            int index = Random.Range(0, spriteList.Count);
            GetComponent<SpriteRenderer>().sprite = spriteList[index];
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        // source = GetComponent<AudioSource>();
        // animator = GetComponent<Animator>();
        // alertDuration = source.clip.length;
        // source.Stop();
        // animator.enabled = false;
    }

    private void Update()
    {
        return;
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
