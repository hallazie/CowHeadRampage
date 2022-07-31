using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackablePawn : MonoBehaviour
{
    // animation event
    public abstract void StartAttack();

    // animation event
    public abstract void StopAttack();

    public abstract void ReceiveDamage(MessageReceiveDamage message);

    public abstract void CauseDamage();

    public abstract void AttackEffect(MessageAttackEffect message);

    public abstract void DamagedEffect(MessageAttackEffect message);

    public abstract void Dead();
}
