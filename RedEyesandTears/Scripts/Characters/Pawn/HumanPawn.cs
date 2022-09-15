using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HumanPawn : MonoBehaviour
{

    public PawnAnimationController animationController;

    // 根据 Hidden State + Outside Input 更新 Pawn 的 Hidden State
    public abstract void UpdateStates();

    // Update：具体的动画
    public abstract void UpdateAnimation();

    // Update：会改变 transform 的变量
    public abstract void UpdateMovement();


    // Update：具体行为的逻辑，不与 Animation 挂钩
    public abstract void UpdateBehaviour();


    public virtual void StopAnimation()
    {
        animationController.StopAnimation();
    }

}
