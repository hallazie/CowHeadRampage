using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SWATAnimationStates
{
    public const string Idle = "Idle";
    public const string Walk = "Walk";
    public const string Shoot = "Shoot";
    public const string Reload = "Reload";
    public const string RifleRun = "RifleRun";
}


public class SWATAnimationController : PawnAnimationController
{
    public SWATController controller;

    public SWATAnimationController(SWATController controller)
    {
        this.controller = controller;
        this.animator = controller.GetComponent<Animator>();
        this.renderer = controller.GetComponent<SpriteRenderer>();
    }

    public override void UpdateAnimationParameter()
    {
        if (!controller.states.alive)
        {
            return;
        }
        else if (controller.states.reloading)
        {
            PlayAnimation(SWATAnimationStates.Reload, true);
        }
        else if (controller.states.shooting && !controller.states.reloading)
        {
            PlayAnimation(SWATAnimationStates.Shoot, true);
        }
        else if (controller.states.moveSpeed > 0)
        {

            PlayAnimation(SWATAnimationStates.RifleRun, true);
        }
        else
        {
            PlayAnimation(SWATAnimationStates.Walk, true);
        }
    }

}
