using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageAttackEffect
{
    public Vector3 origin;
    public Vector3 target;

    public MessageAttackEffect(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
    }
}

public class MessageReceiveDamage
{
    public int damageAmount;

    public MessageReceiveDamage(int damageAmount)
    {
        this.damageAmount = damageAmount;
    }
}
