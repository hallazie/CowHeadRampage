using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfHeadAnimationStates
{
    public string Die = "WH_Die";
    public string FistAttack = "WH_FistAttack";
    public string Idle = "WH_Idle";
    public string Run = "WH_Run";
    public string Sprint = "WH_Sprint";
}

public class WolfHeadAnimationController
{

    public WolfHeadController controller;
    public Animator upperBodyAnimator;

    private WolfHeadAnimationStates animationStates;
    private string currentAnimationName;
    private string previousAnimationName;
    private bool isPlaying;

    public WolfHeadAnimationController(WolfHeadController controller, Animator upperBodyAnimator)
    {
        this.controller = controller;
        this.upperBodyAnimator = upperBodyAnimator;
        this.animationStates = new WolfHeadAnimationStates();
    }

    public void UpdateAnimationParameter()
    {
        if (controller.states.dead)
        {
            PlayAnimation(animationStates.Die);
            return;
        }
        if (controller.states.playerVisible)
        {
            if (controller.states.distance.magnitude > controller.sprintRange)
            {
                PlayAnimation(animationStates.Sprint);
            }
            else if (controller.states.distance.magnitude >= controller.fistAttackRange && controller.states.distance.magnitude < controller.sprintRange)
            {
                PlayAnimation(animationStates.Run);
            }
            else if(controller.states.distance.magnitude < controller.fistAttackRange)
            {
                if (controller.states.allowAttack)
                {
                    PlayAnimation(animationStates.FistAttack);
                }
                else
                {
                    PlayAnimation(animationStates.Idle);
                }
            }
            else
            {
                PlayAnimation(animationStates.Idle);
            }
        }
        else
        {
            PlayAnimation(animationStates.Idle);
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
        if ((currentAnimationName == animationStates.Run || currentAnimationName == animationStates.Idle) && (animationName != animationStates.Run && animationName != animationStates.Idle))
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
