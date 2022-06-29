using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static class CHAnim
{
    public const string Idle = "CH_Idle";
    public const string Run = "CH_Walk";
    public const string AttackWalk = "CH_Attack_Walk";
}


public class CowHeadStatus : PawnStatus
{

    public float horizontalSpeed = 0f;
    public float verticalSpeed = 0f;
    public float speed = 0f;
    public bool attack = false;

    public float health = 10f;

    public Vector2 lookAtPosition = Vector2.zero;

    public void ResolveStatusConflict()
    {
        /*
         * 解决冲突的状态 
         */

    }

}
