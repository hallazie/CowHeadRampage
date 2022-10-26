using UnityEngine;

public abstract class PawnAnimationController
{
    protected string currentAnimationName;
    protected string previousAnimationName;
    protected bool isPlaying;

    protected Animator animator;
    protected SpriteRenderer renderer;

    public abstract void UpdateAnimationParameter();

    public virtual void PlayAnimation(string animationName, bool overwrite = false) {

        renderer.enabled = true;

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

    public virtual void StopAnimation()
    {
        isPlaying = false;
        previousAnimationName = currentAnimationName;
        currentAnimationName = null;
    }
}
