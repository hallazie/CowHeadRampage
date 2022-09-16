using UnityEngine;

public abstract class PawnAnimationController
{
    protected string currentAnimationName;
    protected string previousAnimationName;
    protected bool isPlaying;

    protected Animator animator;

    public abstract void UpdateAnimationParameter();

    public virtual void PlayAnimation(string animationName, bool overwrite = false) {
        isPlaying = true;
        animator.Play(animationName);
        currentAnimationName = animationName;
    }

    public virtual void StopAnimation()
    {
        isPlaying = false;
        previousAnimationName = currentAnimationName;
        currentAnimationName = null;
    }
}
