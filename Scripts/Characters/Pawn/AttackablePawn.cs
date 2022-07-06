using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackablePawn : MonoBehaviour
{
    public abstract void StartAttack();

    public abstract void StopAttack();

    public abstract void ReceiveDamage(MessageReceiveDamage message);

    public abstract void CauseDamage();

    public abstract void AttackEffect(MessageAttackEffect message);

    public abstract void Dead();
}
