using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LonggeStates: EnemyStates
{
    public float health = 10f;

    public Vector3 distance = Vector3.zero;
    public Vector3 target = Vector3.zero;
    public Vector3 headingDirection = Vector3.zero;

    public bool allowAttack = false;

    public string attackMode = "None";
}

public class LonggeController : EnemyController
{

    public Animator animator;
    public Sprite deadSprite;
    public Rigidbody2D rigidbody2d;

    public float walkSpeed = 8f;
    public float runSpeed = 15f;
    public int bleedAmount = 10;

    new public LonggeStates states;

    private float maxHealth;
    private Vector3 originalPosition;
    private Vector3 nextDestination;
    private Vector3 previousPosition;

    private DropdownShadow shadowCaster;

    private void Awake()
    {
        states = new LonggeStates();
        animationController = new LonggeAnimationController(this);
        rigidbody2d = GetComponent<Rigidbody2D>();
        shadowCaster = GetComponent<DropdownShadow>();
        Init(states);
    }

    void Start()
    {
        maxHealth = states.health;
        originalPosition = transform.position;
        
    }

    void Update()
    {
        if (!states.alive)
            return;
        UpdateStates();
        UpdateAnimation();
        UpdateMovement();
        UpdateBehaviour();
    }

    // ===================== Updates Override =====================

    public override void UpdateStates()
    {

    }

    public override void UpdateMovement()
    {
        Vector2 moveDestination = (Vector2)(transform.position - nextDestination);

        if (states.hostileNavQueue == null || moveDestination.magnitude < 0.5f)
        {
            GetRandomPatrolDestination();
            states.navNextPositon = Vector3.zero;
            states.hostileNavQueue = GameManager.instance.gridNavController.FindVertexQueue(transform.position, nextDestination);
            if(states.hostileNavQueue.Count == 0)
            {
                print("empty queue for " + gameObject.name + "!!!");
            }
        }
        else if(states.navNextPositon != Vector3.zero || states.hostileNavQueue != null && states.hostileNavQueue.Count > 0)
        {
            base.TrackPlayer(0.1f, walkSpeed);
        }
    }

    public override void UpdateAnimation()
    {
        animationController.UpdateAnimationParameter();
    }

    public override void UpdateBehaviour()
    {

    }

    // =============================== Control Logics ===============================

    public virtual void GetRandomPatrolDestination()
    {
        // float x = Random.Range(-20f, 20f);
        // float y = Random.Range(-20f, 20f);
        // nextDestination = new Vector3(originalPosition.x + x, originalPosition.y + y, 0);
        nextDestination = GameManager.instance.gridNavController.FindRandomDestination();
    }

    // ===================== Behaviour Events =====================

    public void Dead()
    {
        animator.enabled = false;
        // int deadSpriteIndex = Random.Range(0, deadSpriteList.Count);
        //switch (deadspriteindex)
        //{
        //    case 0:
        //        break;
        //    case 1:
        //        transform.up = states.target.normalized * -1;
        //        break;
        //}
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
        shadowCaster.shadowSize = 0.05f;
        shadowCaster.OffsetShadow();
        GameManager.instance.effectDisplayController.PlayBloodFlow(transform.position, transform.rotation);
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.BroadcastEnemyHostility(range: 100f, requiredVisible: false);

        states.health -= message.damageAmount;
        if (states.health <= 0)
        {
            states.health = 0;
            // animationController.PlayAnimation(WolfHeadAnimationStates.Die, overwrite: true);
            states.alive = false;
            Dead();
        }
    }

    public override void DamagedEffect(MessageAttackEffect message)
    {
        // TODO: use VisionUtil to calc impulse distance and dead sprite
        base.DamagedEffect(message);

        // Vector3 direction = new Vector3(message.target.x - message.origin.x, message.target.y - message.origin.y, 0).normalized;
        Vector3 targetPosition = transform.position + message.damageDirection.normalized * 5f;
        // transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        transform.position = targetPosition;
        GameManager.instance.effectDisplayController.DrawBloodSpread(transform.position, transform.rotation, bleedAmount, 3f);

    }

    public void Respawn()
    {
        states.alive = true;
        states.health = maxHealth;
        animator.enabled = transform;
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = true;
        }    
    }

    // ===================== Animation Events =====================

    // +++++++++++++++++++++ DEBUGS +++++++++++++++++++++++++++++++

    private void OnDrawGizmos()
    {
        return;
        Gizmos.DrawLine(transform.position, nextDestination);
        //if (hostileNavList != null)
        //{
        //    // print("draw nav gizmos with queue size: " + hostileNavList.Count);
        //    Vector3 prevGizmosNav = Vector3.zero;
        //    for (int i = 0; i < hostileNavList.Count; i++)
        //    {
        //        Vector3 nextGizmosNav = hostileNavList[i];
        //        if (prevGizmosNav == Vector3.zero)
        //        {
        //            prevGizmosNav = nextGizmosNav;
        //            continue;
        //        }
        //        else
        //        {
        //            Gizmos.DrawLine((Vector2)prevGizmosNav, (Vector2)nextGizmosNav);
        //        }
        //        prevGizmosNav = nextGizmosNav;
        //    }
        //}
    }
}
