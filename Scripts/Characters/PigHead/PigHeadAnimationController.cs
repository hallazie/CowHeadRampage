using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class PigHeadAnimationStates
{
    public const string Die = "PH_Die";
    public const string Idle = "PH_Idle";
    public const string Run = "PH_Run";
    public const string Sprint = "PH_Sprint";
}

public class PigHeadAnimationController : PawnAnimationController
{
    public PigHeadController controller;
    public Animator animator;

    public PigHeadAnimationController(PigHeadController controller, Animator animator)
    {
        this.controller = controller;
        this.animator = animator;
    }

    public void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            PlayAnimation(PigHeadAnimationStates.Die);
            return;
        }
        else
        {
            PlayAnimation(PigHeadAnimationStates.Idle);
        }
    }

    public override void PlayAnimation(string animationName, bool overwrite = false)
    {
        if ((currentAnimationName == PigHeadAnimationStates.Run || currentAnimationName == PigHeadAnimationStates.Idle) && (animationName != PigHeadAnimationStates.Run && animationName != PigHeadAnimationStates.Idle))
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
