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
    public const string Aim = "WH_Aim";
    public const string Shoot = "WH_Shoot";
}

public class WolfHeadAnimationController: PawnAnimationController
{

    public WolfHeadController controller;
    public Animator upperBodyAnimator;

    public WolfHeadAnimationController(WolfHeadController controller, Animator upperBodyAnimator)
    {
        this.controller = controller;
        this.upperBodyAnimator = upperBodyAnimator;
    }

    public void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            PlayAnimation(WolfHeadAnimationStates.Die, true);
            return;
        }
        if (!GameManager.instance.cowHead.states.alive)
        {
            PlayAnimation(WolfHeadAnimationStates.Idle);
        }
        if (controller.states.playerVisible && controller.states.hostilityLevel == 3)
        {
            // wolfhead进入攻击模式
            if (controller.states.allowAim)
            {
                if (controller.states.allowShoot)
                {
                    PlayAnimation(WolfHeadAnimationStates.Shoot);
                }
                else
                {
                    PlayAnimation(WolfHeadAnimationStates.Aim);
                }
            }
            else
            {
                if (controller.states.distance.magnitude > controller.sprintRange)
                {
                    PlayAnimation(WolfHeadAnimationStates.Sprint);
                }
                else if (controller.states.distance.magnitude >= controller.fistAttackRange && controller.states.distance.magnitude < controller.sprintRange)
                {
                    PlayAnimation(WolfHeadAnimationStates.Run);
                }
                else if (controller.states.distance.magnitude < controller.fistAttackRange)
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
        }
        else
        {
            PlayAnimation(WolfHeadAnimationStates.Idle);
        }
    }

    public override void PlayAnimation(string animationName, bool overwrite = false)
    {
        // Debug.Log("playing: " + animationName + ", current: " + currentAnimationName + ", previous: " + previousAnimationName);
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
