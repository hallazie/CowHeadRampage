using UnityEngine;

public class MessageInstantiateBullet
{

    public string targetTag;
    public string sourceTag;
    public Vector3 targetPosition;
    public Vector3 sourcePosition;
    public float damageAmount;

    public MessageInstantiateBullet(string targetTag, string sourceTag, Vector3 targetPosition, Vector3 sourcePosition, float damageAmount)
    {
        this.targetTag = targetTag;
        this.sourceTag = sourceTag;
        this.targetPosition = targetPosition;
        this.sourcePosition = sourcePosition;
        this.damageAmount = damageAmount;
    }

}


public class MessageAttackEffect
{
    public Vector3 origin;
    public Vector3 target;
    public Vector3 contactPosition;

    public MessageAttackEffect(Vector3 origin, Vector3 target, Vector3 contactPosition)
    {
        this.origin = origin;
        this.target = target;
        this.contactPosition = contactPosition;
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

