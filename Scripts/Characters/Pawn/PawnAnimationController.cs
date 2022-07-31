using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PawnAnimationController
{
    protected string currentAnimationName;
    protected string previousAnimationName;
    protected bool isPlaying;

    public abstract void PlayAnimation(string animationName, bool overwrite = false);

    public void StopAnimation()
    {
        isPlaying = false;
        previousAnimationName = currentAnimationName;
        currentAnimationName = null;
    }

}
