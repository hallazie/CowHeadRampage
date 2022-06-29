using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class RKAnim
{
    public const string Idle = "RK_Idle";
    public const string Run = "RK_Run";
    public const string Melee = "RK_Melee";
}

class RKStatus
{
    public float horizontalSpeed = 0f;
    public float verticalSpeed = 0f;
    public bool melee = false;

    public void ResolveStatusConflict()
    {
        /*
         * 解决冲突的状态 
         */
        
    }
}


public class MovementController : MonoBehaviour
{

    private Vector3 transformNormal = new Vector3(1, 1, 1);
    private Vector3 transformMirror = new Vector3(-1, 1, 1);
    private RKStatus status = new RKStatus();
    private string prevAnimation;

    public Animator animator;
    public float Speed = 5f;

    public int meleeRate = 2;
    private float nextMeleeTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        status.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        status.verticalSpeed = Input.GetAxisRaw("Vertical");
        status.melee = Input.GetMouseButtonDown(0) || status.melee;

        UpdateRKStatus();
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(status.horizontalSpeed * Time.deltaTime * Speed, status.verticalSpeed * Time.deltaTime * Speed, 0);
    }

    private void UpdateRKStatus()
    {
        /*
         使用script替代animation状态机，获得更好的控制
         */
        status.ResolveStatusConflict();
        UpdateRKDirection();
        if (status.horizontalSpeed != 0)
        {
            status.melee = false;
            PlayAnimation(RKAnim.Run);
        }
        else if (status.melee)
        {
            PlayAnimation(RKAnim.Melee);
        }
        else
        {
            PlayAnimation(RKAnim.Idle);
        }
    }

    public void AnimationEventListener(string eventString)
    {
        if (eventString == "meleeEnd")
        {
            status.melee = false;
        }
    }

    private void PlayAnimation(string currentAnimation, bool overwrite=false)
    {
        if (currentAnimation == prevAnimation && !overwrite)
            return;
        animator.Play(currentAnimation);
        prevAnimation = currentAnimation;
    }

    private void UpdateRKDirection()
    {
        if (status.horizontalSpeed > 0)
        {
            transform.localScale = transformNormal;
        }
        else if (status.horizontalSpeed < 0)
        {
            transform.localScale = transformMirror;
        }
    }
}
