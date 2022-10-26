using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static class PlayerAnimationStates
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

public class PlayerAnimator : PawnAnimationController
{
    public PlayerController controller;
    private Animator shadowAnimator;

    private List<string> unOverwritable = new List<string> {
        PlayerAnimationStates.RightCross,
        PlayerAnimationStates.LeftCross,
        PlayerAnimationStates.FlashStart
    };

    public PlayerAnimator(PlayerController controller, Animator shadowAnimator)
    {
        this.controller = controller;
        this.animator = controller.GetComponent<Animator>();
        this.renderer = controller.GetComponent<SpriteRenderer>();
        this.shadowAnimator = shadowAnimator;
    }

    public override void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            return;
        }
        else if (controller.states.flash)
        {
            PlayAnimation(PlayerAnimationStates.FlashStart, true);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 0)
        {
            PlayAnimation(PlayerAnimationStates.LeftCross, false);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 1)
        {
            PlayAnimation(PlayerAnimationStates.RightCross, false);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 2)
        {
            PlayAnimation(PlayerAnimationStates.LeftHook, false);
            return;
        }
        else if (controller.states.meleeAttacking && controller.states.comboStep == 3)
        {
            PlayAnimation(PlayerAnimationStates.RightHook, false);
            return;
        }
        else if (controller.states.moving)
        {
            PlayAnimation(PlayerAnimationStates.Walk);

        }
        else if (!controller.states.moving)
        {

            PlayAnimation(PlayerAnimationStates.Idle, true);
        }
        else
        {


            PlayAnimation(PlayerAnimationStates.Idle);
            
        }
    }

    public override void PlayAnimation(string animationName, bool overwrite = false)
    {
        if ((currentAnimationName == PlayerAnimationStates.Walk || currentAnimationName == PlayerAnimationStates.Idle) && (animationName != PlayerAnimationStates.Walk && animationName != PlayerAnimationStates.Idle))
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
        shadowAnimator.Play(animationName);
        currentAnimationName = animationName;
    }
}
