using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLowerBodyAnimationStates
{
    public const string Move = "LowerbodyMove";
    public const string Idle = "LowerbodyIdle";
    public const string Boxing = "LowerbodyBoxing";
}


public class PlayerLowerBodyAnimator : PawnAnimationController
{
    public PlayerLowerBodyController controller;

    private float lastAttackTime;
    private bool boxingMove;
    private float boxingMoveGap = 0.1f;

    public PlayerLowerBodyAnimator(PlayerLowerBodyController controller)
    {
        this.controller = controller;
        this.animator = controller.GetComponent<Animator>();
        this.renderer = controller.GetComponent<SpriteRenderer>();
    }

    public override void UpdateAnimationParameter()
    {
        if (controller.player.states.meleeAttacking)
        {
            lastAttackTime = Time.time;
        }
        // boxingMove = (Time.time - lastAttackTime) < boxingMoveGap;
        boxingMove = controller.player.states.meleeAttacking;

        if (controller.player.states.flash)
        {
            // PlayAnimation(PlayerLowerBodyAnimationStates.Idle, true);
            renderer.enabled = false;
        }
        else if (boxingMove && !controller.player.states.moving)
        {
            // 原地打拳
            PlayAnimation(PlayerLowerBodyAnimationStates.Boxing, true);
            // renderer.enabled = false;
        }
        else if (boxingMove && controller.player.states.moving)
        {
            // 边走边打
            PlayAnimation(PlayerLowerBodyAnimationStates.Boxing, true);
        }
        else if (!boxingMove && controller.player.states.moving)
        {
            PlayAnimation(PlayerLowerBodyAnimationStates.Move, true);
        }
        else
        {
            // PlayAnimation(PlayerLowerBodyAnimationStates.Idle, true);
            renderer.enabled = false;
        }
    }

}
