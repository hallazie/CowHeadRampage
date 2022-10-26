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

    public LonggeAnimationController(LonggeController controller)
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
