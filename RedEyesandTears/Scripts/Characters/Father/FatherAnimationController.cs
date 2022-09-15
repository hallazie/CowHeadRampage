using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static class FatherAnimationStates
{
    public const string Idle = "FatherIdle";
    public const string Walk = "FatherWalk";
    public const string LeftCross = "FatherCrossLeft";
    public const string RightCross = "FatherCrossRight";

}

public class FatherAnimationController : PawnAnimationController
{
    public FatherController controller;

    public FatherAnimationController(FatherController controller, Animator animator)
    {
        this.controller = controller;
        this.animator = animator;
    }

    public override void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            return;
        }
        if (controller.states.attack && controller.states.comboStep == 0)
        {
            PlayAnimation(FatherAnimationStates.LeftCross, false);
            return;
        }
        else if (controller.states.attack && controller.states.comboStep == 1)
        {
            PlayAnimation(FatherAnimationStates.RightCross, false);
            return;
        }
        else if (controller.states.moveSpeed > 0)
        {

            PlayAnimation(FatherAnimationStates.Walk);
        }
        else
        {
            PlayAnimation(FatherAnimationStates.Idle);
        }
    }

    public override void PlayAnimation(string animationName, bool overwrite = false)
    {
        if ((currentAnimationName == FatherAnimationStates.Walk || currentAnimationName == FatherAnimationStates.Idle) && (animationName != FatherAnimationStates.Walk && animationName != FatherAnimationStates.Idle))
        {
            overwrite = true;
        }
        if (animationName != currentAnimationName)
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
        animator.Play(animationName);
        currentAnimationName = animationName;
    }
}
