using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LonggeStates: EnemyStates
{
    public float health = 10f;
    public float moveSpeed = 0f;

    public Vector3 distance = Vector3.zero;
    public Vector3 target = Vector3.zero;
    public Vector3 headingDirection = Vector3.zero;

    public bool allowAttack = false;
    public bool lostTrack = false;

    public string attackMode = "None";
}

public class LonggeController : EnemyController
{

    public Animator animator;
    public Sprite deadSprite;

    public float walkSpeed = 8f;
    public float runSpeed = 15f;

    new public LonggeStates states;

    private float maxHealth;
    private Vector3 originalPosition;
    private Vector3 nextDestination;
    private Vector3 previousPosition;

    private void Awake()
    {
        states = new LonggeStates();
        animationController = new LonggeAnimationController(this, animator);
    }

    void Start()
    {
        maxHealth = states.health;
        Init();
    }

    public override void Init()
    {
        base.Init();
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
        if (moveDestination.magnitude < 0.1f)
        {
            GetRandomPatrolDestination();
        }
        else
        {
            Vector2 heading = -1 * moveDestination.normalized;
            Vector3 movement = new Vector3(heading.x * walkSpeed * Time.deltaTime, heading.y * walkSpeed * Time.deltaTime, 0);
            transform.up = movement.normalized;
            transform.position += movement;
        }
        if ((transform.position - previousPosition).magnitude < 0.001f)
        {
            GetRandomPatrolDestination();
        }
        previousPosition = transform.position;
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
        float x = Random.Range(-20f, 20f);
        float y = Random.Range(-20f, 20f);
        nextDestination = new Vector3(originalPosition.x + x, originalPosition.y + y, 0);
    }

    // ===================== Behaviour Events =====================

    public void Dead()
    {
        return;
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
        // rgdbody.AddForce((transform.position - cowHead.transform.position).normalized * 5f);
        // StartCoroutine(SlideOnDirection(transform.position - cowHead.transform.position, 5f));
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        return;
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
        base.DamagedEffect(message);
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
        }    }

    // ===================== Animation Events =====================
}
