using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static class CowHeadAnimationStates
{
    public const string Idle = "CH_Idle";
    public const string Run = "CH_Walk";
    public const string AttackWalk = "CH_Attack_Walk";
    public const string Die = "CH_Die";
}


public class CowHeadAnimationController
{

    public CowHeadController controller;
    public Animator upperBodyAnimator;

    private string currentAnimationName;
    private string previousAnimationName;
    private bool isPlaying;

    public CowHeadAnimationController(CowHeadController controller, Animator upperBodyAnimator)
    {
        this.controller = controller;
        this.upperBodyAnimator = upperBodyAnimator;
    }

    public void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            return;
        }
        if (controller.states.attack)
        {
            PlayAnimation(CowHeadAnimationStates.AttackWalk, true);
            return;
        }
        if (controller.states.moveSpeed > 0)
        {
            PlayAnimation(CowHeadAnimationStates.Run);
        }
        else
        {
            PlayAnimation(CowHeadAnimationStates.Idle);
        }

    }

    public void StopAnimation()
    {
        isPlaying = false;
        previousAnimationName = currentAnimationName;
        currentAnimationName = null;
    }

    public void PlayAnimation(string animationName, bool overwrite = false)
    {
        if ((currentAnimationName == CowHeadAnimationStates.Run || currentAnimationName == CowHeadAnimationStates.Idle) && (animationName != CowHeadAnimationStates.Run && animationName != CowHeadAnimationStates.Idle))
        {
            overwrite = true;
        }
        if (isPlaying && !overwrite)
        {
            return;
        }
        if (animationName == currentAnimationName && !overwrite)
        {
            return;
        }
        isPlaying = true;
        upperBodyAnimator.Play(animationName);
        currentAnimationName = animationName;
    }
}
