using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyStates {
    public int hostilityLevel = 1;                   // 0: friend, 1: neutral, 2: hostile, 3: enemy

    public bool alive = true;
    public bool playerVisible = false;
    public bool lostTrack = false;

    public float moveSpeed = 0f;

    public Vector3 navNextPositon = Vector3.zero;
    public Vector3 navNextTarget = Vector3.zero;
    public Queue<Vector3> hostileNavQueue = null;
    public Queue<Vector3> patrolNavQueue = null;
    public Queue<Vector3> patrolTargetQueue = null;
}

public class EnemyController : HumanPawn
{

    public EnemyStates states;

    public virtual void Init(EnemyStates states)
    {
        this.states = states;
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

    public void CollideEachOther(bool collide, string tag)
    {
        Physics2D.IgnoreLayerCollision(GameManager.instance.layerDict[tag], GameManager.instance.layerDict[tag], ignore: collide);
    }

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

    public virtual void TrackPlayer(float minNavDestinationDist, float moveSpeed)
    {
        // Physics2D.IgnoreLayerCollision(GameManager.instance.layerDict["Enemy"], GameManager.instance.layerDict["Enemy"], ignore: true);
        CollideEachOther(false, "Enemy");
        Vector2 navMovePosition = (Vector2)(transform.position - states.navNextPositon);
        if (states.navNextPositon == Vector3.zero || navMovePosition.magnitude <= minNavDestinationDist)
        {
            if(states.hostileNavQueue != null && states.hostileNavQueue.Count > 0)
            {
                states.navNextPositon = states.hostileNavQueue.Dequeue();
            }
            else
            {
                states.lostTrack = true;
                states.moveSpeed = 0;
                CollideEachOther(true, "Enemy");
                states.navNextPositon = Vector3.zero;
            }
        }
        else
        {
            states.lostTrack = false;
            Vector2 heading = (states.navNextPositon - transform.position);
            Vector2 headingNorm = heading.normalized;
            Vector3 target = new Vector3(headingNorm.x * moveSpeed * Time.deltaTime, headingNorm.y * moveSpeed * Time.deltaTime, 0);
            if (heading.magnitude > 0.01f)
            {
                transform.up = target.normalized;
                transform.position += target;
                states.moveSpeed = moveSpeed;
            }
        }
    }

    public virtual void FindPatrolTarget(List<Vector2> patrolList)
    {
        if (states.patrolTargetQueue == null)
        {
            states.patrolTargetQueue = new Queue<Vector3>();
            states.patrolTargetQueue.Clear();
            foreach (Vector2 patrolPosition in patrolList)
            {
                states.patrolTargetQueue.Enqueue(patrolPosition);
            }
        }
        if (states.patrolTargetQueue != null && states.patrolTargetQueue.Count == 0)
        {
            states.patrolTargetQueue.Clear();
            foreach (Vector2 patrolPosition in patrolList)
            {
                states.patrolTargetQueue.Enqueue(patrolPosition);
            }
        }
        if (states.navNextTarget == Vector3.zero)
        {
            states.navNextTarget = states.patrolTargetQueue.Dequeue();
            states.patrolNavQueue = GameManager.instance.gridNavController.FindVertexQueue(transform.position, states.navNextTarget);
        }
    }

    public virtual void Patrol(float minNavDestinationDist, float moveSpeed)
    {
        CollideEachOther(false, "Enemy");
        Vector2 navMovePosition = (Vector2)(transform.position - states.navNextPositon);
        if ((states.navNextPositon == Vector3.zero || navMovePosition.magnitude <= minNavDestinationDist) && states.patrolNavQueue != null && states.patrolNavQueue.Count > 0)
        {
            // 未走完当前nav target queue，但走完单个cell
            states.navNextPositon = states.patrolNavQueue.Dequeue();
        }
        else if (navMovePosition.magnitude <= minNavDestinationDist && (states.patrolNavQueue != null && states.patrolNavQueue.Count == 0 || states.patrolNavQueue == null))
        {
            // 走完当前nav target queue
            if(states.navNextTarget != Vector3.zero)
            {
                states.navNextTarget = Vector3.zero;
                states.navNextPositon = Vector3.zero;
            }
            else
            {
                states.patrolNavQueue = GameManager.instance.gridNavController.FindVertexQueue(transform.position, states.navNextTarget);
            }
        }
        else if (states.navNextPositon != Vector3.zero)
        {
            Vector2 heading = (states.navNextPositon - transform.position);
            Vector2 headingNorm = heading.normalized;
            Vector3 target = new Vector3(headingNorm.x * moveSpeed * Time.deltaTime, headingNorm.y * moveSpeed * Time.deltaTime, 0);
            if (heading.magnitude > 0.01f)
            {
                transform.up = target.normalized;
                transform.position += target;
                states.moveSpeed = moveSpeed;
            }
        }
    }
}
