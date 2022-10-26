using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLowerBodyController : MonoBehaviour
{

    public Animator animator;
    public PlayerController player;
    public PlayerLowerBodyAnimator animationController;

    private Vector2 lastDirection;

    private void Awake()
    {
        animationController = new PlayerLowerBodyAnimator(this);
    }

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }


    void Update()
    {
        if (player.states.meleeAttacking)
        {
            transform.localPosition = new Vector2(0, 0.8f);
        }
        else
        {
            transform.localPosition = new Vector2(0, -0.3f);
        }

        float moveLookAngle = Vector2.Angle(player.states.lookAtDirection, player.states.movementDirection);
        if (player.states.moving)
        {
            transform.up = player.states.movementDirection;
            lastDirection = player.states.movementDirection;
        }
        else
        {
            transform.up = lastDirection;
        }
        // print("lookat: " + player.states.lookAtDirection + ", moveto: " + player.states.movementDirection + ", angle: " + moveLookAngle);
        animationController.UpdateAnimationParameter();
    }

    public void StopAnimation()
    {
        animationController.StopAnimation();
    }
}
