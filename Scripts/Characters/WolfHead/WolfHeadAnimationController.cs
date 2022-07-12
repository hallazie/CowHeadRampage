using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class WolfHeadAnimationStates
{
    public const string Die = "WH_Die";
    public const string FistAttack = "WH_FistAttack";
    public const string Idle = "WH_Idle";
    public const string Run = "WH_Run";
    public const string Sprint = "WH_Sprint";
}

public class WolfHeadAnimationController
{

    public WolfHeadController controller;
    public Animator upperBodyAnimator;

    private string currentAnimationName;
    private string previousAnimationName;
    private bool isPlaying;

    public WolfHeadAnimationController(WolfHeadController controller, Animator upperBodyAnimator)
    {
        this.controller = controller;
        this.upperBodyAnimator = upperBodyAnimator;
    }

    public void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            PlayAnimation(WolfHeadAnimationStates.Die);
            return;
        }
        if (controller.states.playerVisible)
        {
            if (controller.states.distance.magnitude > controller.sprintRange)
            {
                PlayAnimation(WolfHeadAnimationStates.Sprint);
            }
            else if (controller.states.distance.magnitude >= controller.fistAttackRange && controller.states.distance.magnitude < controller.sprintRange)
            {
                PlayAnimation(WolfHeadAnimationStates.Run);
            }
            else if(controller.states.distance.magnitude < controller.fistAttackRange)
            {
                if (controller.states.allowAttack)
                {
                    PlayAnimation(WolfHeadAnimationStates.FistAttack);
                }
                else
                {
                    PlayAnimation(WolfHeadAnimationStates.Idle);
                }
            }
            else
            {
                PlayAnimation(WolfHeadAnimationStates.Idle);
            }
        }
        else
        {
            PlayAnimation(WolfHeadAnimationStates.Idle);
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
        if ((currentAnimationName == WolfHeadAnimationStates.Run || currentAnimationName == WolfHeadAnimationStates.Idle) && (animationName != WolfHeadAnimationStates.Run && animationName != WolfHeadAnimationStates.Idle))
        {
            overwrite = true;
        }
        if (isPlaying && !overwrite)
        {
            return;
        }
        if(animationName == currentAnimationName && !overwrite)
        {
            return;
        }
        isPlaying = true;
        upperBodyAnimator.Play(animationName);
        currentAnimationName = animationName;
    }
}
