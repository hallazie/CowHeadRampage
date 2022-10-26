using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTimeEffectItem : MonoBehaviour
{

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private string[] clipList;
    private int clipSize;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        clipSize = animator.runtimeAnimatorController.animationClips.Length;
        clipList = new string[clipSize];
        for (int i = 0; i < clipSize; i++)
        {
            clipList[i] = animator.runtimeAnimatorController.animationClips[i].name;
        }
        // animator.enabled = false;
        // spriteRenderer.enabled = false;
    }

    public void Init()
    {
        animator.enabled = true;
        spriteRenderer.enabled = true;
    }

    public void RandomPlay()
    {
        int index = Random.Range(0, clipSize);
        string name = clipList[index];
        animator.Play(name);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}