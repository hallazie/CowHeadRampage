using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FatherState
{

    public bool alive = true;
    public bool occupied = false;
    public bool freezeMovement = false;
    public bool attack = false;
    public bool sprint = false;
    public bool weave = false;
    public bool invincible = false;
    public bool idleBoxing = false;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float moveSpeed;
    public float health;
    public float lastAttackTime = 0f;
    public float lastBoxingTime = 0f;

    public Vector2 lookAtDirection;
    public Vector2 movementDirection;

    public int comboStep;

    public string weaveType = null;

}

public class FatherController : HumanPawn
{

    public Animator animator;

    public FatherState states;
    public MeleeWeapon weapon;

    // =================================== Basic Attributes ===================================
    public int attackDamage = 10;
    public float walkSpeed = 15f;

    private void Awake()
    {
        states = new FatherState();
        weapon = GetComponentInChildren<MeleeWeapon>();
        weapon.Init(attackDamage: attackDamage, fontColor: Color.red);

        animationController = new FatherAnimationController(this, animator);
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!states.alive || states.occupied)
            return;
        UpdateStates();
        UpdateAnimation();
        UpdateMovement();
        UpdateBehaviour();
    }

    public override void UpdateStates()
    {
        bool inputAttack = Input.GetMouseButton(0) || Input.GetMouseButtonDown(0);
        states.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        states.verticalSpeed = Input.GetAxisRaw("Vertical");

        states.moveSpeed = Mathf.Sqrt(states.horizontalSpeed * states.horizontalSpeed + states.verticalSpeed * states.verticalSpeed);

        states.lookAtDirection = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        states.movementDirection = new Vector2(states.horizontalSpeed, states.verticalSpeed).normalized;

        states.attack = inputAttack || states.attack;
        
    }

    public override void UpdateAnimation()
    {
        animationController.UpdateAnimationParameter();
    }

    public override void UpdateMovement()
    {
        /*
         update movements in fixed update
         */

        if (states.occupied || states.freezeMovement)
            return;
        if (!states.sprint)
        {
            gameObject.transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * walkSpeed, states.verticalSpeed * Time.deltaTime * walkSpeed, 0);
        }
        else
        {
            // gameObject.transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, states.verticalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, 0);
        }
        transform.up = states.lookAtDirection;
    }

    public override void UpdateBehaviour()
    {
        if (states.attack)
        {
            CauseDamage();
        }
    }

    // =================================== Behaviour Events ===================================

    public void CauseDamage()
    {
        weapon.OnAttack("Enemy", showAttackEffect: true, ignoringTags: new List<string> { "Bullet", "Weapon", "Player" });
        weapon.OnAttack("Bullet", sendDamagedEffect: false, showDamage: false, visualConditional: false);
        weapon.OnAttack("Vehicle", sendDamage: true, sendDamagedEffect: true, showDamage: false, showAttackEffect: true, visualConditional: false);
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        if (states.weave && states.invincible)
        {
            return;
        }
        states.health -= message.damageAmount;
        // FloatingTextManager.instance.ShowBasic("-" + message.damageAmount.ToString(), Color.red, gameObject.transform.position, Vector3.up * 64, duration: 2f, fontSize: 32);
        if (states.health <= 0)
        {
            states.health = 0;
            states.alive = false;
            GameManager.instance.playerAlive = false;
            // animationController.PlayAnimation(FatherAnimationStates.Die, overwrite: true);
        }
        // FloatingTextManager.instance.ShowBasic("health: " + health.ToString(), Color.yellow, transform.position, Vector3.up * 32, duration: 5f);
    }

    public void AttackEffect(MessageAttackEffect message)
    {
        // 只播放挥动音效。实际击打音效由受击打物确定（在ReceiveDamage中播放）
        GameManager.instance.SoundController.PlaySound(SoundNames.FatherPunch);
    }

    public void DamagedEffect(MessageAttackEffect message)
    {
        if (states.weave && states.invincible)
        {
            return;
        }
    }

    public void Dead()
    {
        states.alive = false;
        animator.enabled = false;
        GameManager.instance.BroadcastEnemyHostility(1, 1000, false);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        // gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
    }

    // =================================== Animation Events ===================================

    public override void StopAnimation()
    {
        UnfreezeMovement();
        animationController.StopAnimation();
    }

    public void FreezeMovement()
    {
        states.freezeMovement = true;
        if (states.weave)
        {
            states.invincible = true;
        }
        else
        {
            states.invincible = false;
        }
    }

    public void UnfreezeMovement()
    {
        states.freezeMovement = false;
        if (states.weave)
        {
            states.invincible = false;
        }
    }

    public void StopAttack()
    {
        states.attack = false;
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
        float randomIndex = Random.Range(0f, 1f);
        if (randomIndex > 0.5)
        {
            states.comboStep = 1;
        }
        else
        {
            states.comboStep = 0;
        }
        states.idleBoxing = true;
        states.lastBoxingTime = Time.time;
    }

    public void StartAttack()
    {
        states.weave = false;
        states.attack = true;
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

}
