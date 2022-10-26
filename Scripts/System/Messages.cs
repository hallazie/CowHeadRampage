using UnityEngine;

public class MessageInstantiateBullet
{
    /*
     子弹
     */

    public string targetTag;
    public string sourceTag;
    public Vector3 targetPosition;
    public Vector3 sourcePosition;
    public float damageAmount;
    public float jitter;            // 0: perfect accurate, 1: spread 180 degree

    public MessageInstantiateBullet(string targetTag, string sourceTag, Vector3 targetPosition, Vector3 sourcePosition, float damageAmount, float jitter=0f)
    {
        this.targetTag = targetTag;
        this.sourceTag = sourceTag;
        this.targetPosition = targetPosition;
        this.sourcePosition = sourcePosition;
        this.damageAmount = damageAmount;
        this.jitter = jitter;
    }

}


public class MessageAttackEffect
{

    /*
     物体被击打时播放何种效果
     */

    public Vector3 origin;
    public Vector3 target;
    public Vector3 contactPosition;
    public Vector3 damageDirection;

    public MessageAttackEffect(Vector3 origin, Vector3 target, Vector3 contactPosition, Vector3 damageDirection)
    {
        this.origin = origin;
        this.target = target;
        this.contactPosition = contactPosition;
        this.damageDirection = damageDirection;
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


public class MessageThrowItem
{
    public Sprite sprite;
    public int damageAmount;
    public Vector2 throwDirection;
    public float itemMass;
    public float throwPower;

    public MessageThrowItem(Sprite sprite, int damageAmount, Vector2 throwDirection, float itemMass, float throwPower)
    {
        this.sprite = sprite;
        this.damageAmount = damageAmount;
        this.throwDirection = throwDirection;
        this.itemMass = itemMass;
        this.throwPower = throwPower;
    }
}


public class MessagePlayEffect
{
    public Vector3 position;
    public Vector3 rotation;
    public float radius;
    public string effectName;

    public MessagePlayEffect(Vector3 position, Vector3 rotation, float radius=0f, string effectName="None")
    {
        this.position = position;
        this.rotation = rotation;
        this.radius = radius;
        this.effectName = effectName;
    }
}

