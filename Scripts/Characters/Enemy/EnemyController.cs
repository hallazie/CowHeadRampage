using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyStates {
    public int hostilityLevel = 1;                   // 0: friend, 1: neutral, 2: hostile, 3: enemy

    public bool alive = true;
    public bool playerVisible = false;
}

public class EnemyController : HumanPawn
{

    public EnemyStates states = new EnemyStates();

    public virtual void Init()
    {

    }

    public override void UpdateAnimation()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateMovement()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateStates()
    {
        throw new System.NotImplementedException();
    }


    // =============================== Behaviour Events ===============================

    public virtual void DamagedEffect(MessageAttackEffect message)
    {
        Vector3 direction = (message.origin - message.target);
        direction.z = 0;
        direction = direction.normalized;
        Vector3 position = new Vector3(message.target.x - direction.x * 0.3f, message.target.y - direction.y * 0.3f, 0);
        Vector3 contactProximatePosition = (message.target + message.origin) / 2f;

        float jitterX = Random.Range(-0.5f, 0.5f);
        float jitterY = Random.Range(-0.5f, 0.5f);

        GameManager.instance.effectDisplayController.PlayMeatImpactEffect(new Vector3(transform.position.x + jitterX, transform.position.y + jitterY, 0));
        GameManager.instance.ShakeCamera();
    }
}
