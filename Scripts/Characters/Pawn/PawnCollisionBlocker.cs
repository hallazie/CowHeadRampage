using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnCollisionBlocker : MonoBehaviour
{

    public BoxCollider2D pawnCollider;
    public BoxCollider2D pawnBlockerCollider;

    private Vector3 origin;

    // 防止player、enemy相互push
    void Start()
    {
        Physics2D.IgnoreCollision(pawnCollider, pawnBlockerCollider, true);
        origin = transform.position;
    }

}
