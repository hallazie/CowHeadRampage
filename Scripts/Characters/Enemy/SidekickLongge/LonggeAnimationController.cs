using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LonggeAnimationStates
{
    public const string Idle = "LonggeIdle";
    public const string Walk = "LonggeWalk";
}


public class LonggeAnimationController : PawnAnimationController
{

    public LonggeController controller;

    public LonggeAnimationController(LonggeController controller, Animator animator)
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
        else if (controller.states.moveSpeed > 0)
        {

            PlayAnimation(LonggeAnimationStates.Walk);
        }
        else
        {
            PlayAnimation(LonggeAnimationStates.Walk);
        }
    }

}
