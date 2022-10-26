using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static class FatherAnimationStates
{
    // public const string Idle = "FatherIdle";
    public const string Idle = "FatherIdleNude";
    public const string GrabItenIdle = "FatherIdleGrab";
    public const string Walk = "FatherWalk";
    public const string GrabItemWalk = "FatherGrabItemWalk";
    public const string GrabItem = "FatherGrabItem";
    public const string ThrowItem = "FatherThrowItem";

    // public const string LeftCross = "FatherCrossLeft";
    // public const string RightCross = "FatherCrossRight";
    // public const string LeftHook = "FatherHookLeft";
    // public const string RightHook = "FatherHookRight";
    public const string LeftCross = "PlayerNudeJab";
    public const string RightCross = "PlayerNudeCross";
    public const string LeftHook = "PlayerNudeLeftHook";
    public const string RightHook = "PlayerNudeRightHook";

    public const string FlashStart = "FatherFlashStart";

}

public class FatherAnimationController : PawnAnimationController
{
    public FatherController controller;

    private List<string> unOverwritable = new List<string> { 
        FatherAnimationStates.RightCross,
        FatherAnimationStates.LeftCross,
        FatherAnimationStates.FlashStart
    };

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
        else if (controller.states.tryGrab && !controller.states.grabbingItem)
        {
            PlayAnimation(FatherAnimationStates.GrabItem);
        }
        else if (controller.states.throwing)
        {
            PlayAnimation(FatherAnimationStates.ThrowItem);
        }
        else if (controller.states.flash)
        {
            PlayAnimation(FatherAnimationStates.FlashStart, true);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 0)
        {
            PlayAnimation(FatherAnimationStates.LeftCross, false);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 1)
        {
            PlayAnimation(FatherAnimationStates.RightCross, false);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 2)
        {
            PlayAnimation(FatherAnimationStates.LeftHook, false);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 3)
        {
            PlayAnimation(FatherAnimationStates.RightHook, false);
            return;
        }
        else if (controller.states.moving)
        {
            if (controller.states.grabbingItem)
            {
                PlayAnimation(FatherAnimationStates.GrabItemWalk);
            }
            else
            {
                PlayAnimation(FatherAnimationStates.Walk);
            }
        }
        else if (!controller.states.moving)
        {
            if (controller.states.grabbingItem)
            {
                PlayAnimation(FatherAnimationStates.GrabItenIdle, true);
            }
            else
            {
                PlayAnimation(FatherAnimationStates.Idle, true);
            }
        }
        else
        {
            if (controller.states.grabbingItem)
            {
                PlayAnimation(FatherAnimationStates.GrabItenIdle);
            }
            else
            {
                PlayAnimation(FatherAnimationStates.Idle);
            }
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
            if (unOverwritable.Contains(currentAnimationName))
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
        animator.Play(animationName);
        currentAnimationName = animationName;
    }
}
