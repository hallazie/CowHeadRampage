using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static class CowHeadAnimationStates
{
    public const string Idle = "CH_Idle";
    public const string Run = "CH_Walk";
    public const string Sprint = "CH_Sprint";
    public const string AttackWalk = "CH_Attack_Walk";
    public const string Die = "CH_Die";

    public const string Combo1 = "CH_Combo1";
    public const string Combo2 = "CH_Combo2";
}


public class CowHeadAnimationController: PawnAnimationController
{

    public CowHeadController controller;
    public Animator upperBodyAnimator;

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
        if (controller.states.attack && controller.states.comboStep == 0)
        {
            PlayAnimation(CowHeadAnimationStates.Combo1, false);
            return;
        }
        else if(controller.states.attack && controller.states.comboStep == 1)
        {
            PlayAnimation(CowHeadAnimationStates.Combo2, false);
            return;
        }
        if (controller.states.moveSpeed > 0)
        {
            if (controller.states.sprint)
            {
                PlayAnimation(CowHeadAnimationStates.Sprint);
            }
            else
            {
                PlayAnimation(CowHeadAnimationStates.Run);
            }
        }
        else
        {
            PlayAnimation(CowHeadAnimationStates.Idle);
        }
    }

    public override void PlayAnimation(string animationName, bool overwrite = false)
    {
        if ((currentAnimationName == CowHeadAnimationStates.Run || currentAnimationName == CowHeadAnimationStates.Idle) && (animationName != CowHeadAnimationStates.Run && animationName != CowHeadAnimationStates.Idle))
        {
            overwrite = true;
        }
        if (animationName != currentAnimationName)
        {
            if(animationName == CowHeadAnimationStates.Combo1 && currentAnimationName == CowHeadAnimationStates.Combo2 || animationName == CowHeadAnimationStates.Combo2 && currentAnimationName == CowHeadAnimationStates.Combo1)
            {
                overwrite = false;
            }
            else
            {
                overwrite = true;
            }
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
