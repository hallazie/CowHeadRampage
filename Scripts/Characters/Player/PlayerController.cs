using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{

    public bool alive = true;
    public bool occupied = false;
    public bool freezeMovement = false;
    public bool moving = false;
    public bool meleeAttacking = false;
    public bool flash = false;
    public bool sprint = false;
    public bool weaving = false;
    public bool invincible = false;
    public bool idleBoxing = false;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float moveSpeed;
    public float health;

    public float lastAttackTime = 0f;
    public float lastFlashTime = 0f;

    public Vector2 lookAtDirection;
    public Vector2 movementDirection;
    public Vector2 flashDirection;

    public int comboStep;

    public string weaveType = null;

}

public class PlayerController : HumanPawn
{

    public Animator animator;
    public Animator shadowAnimator;

    public PlayerState states;
    public MeleeWeapon weapon;

    public int attackDamage = 10;
    public float walkSpeed = 15f;
    public float flashGap = 2f;
    public float flashDistance = 5f;
    public float flashSpeed = 10f;

    private Vector3 flashDestiny;

    private void Awake()
    {
        states = new PlayerState();
        weapon = GetComponentInChildren<MeleeWeapon>();
        weapon.Init(attackDamage: attackDamage, fontColor: Color.red);

        animationController = new PlayerAnimator(this, shadowAnimator);

    }

    void Start()
    {
        states.moveSpeed = walkSpeed;
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
        bool inputFlash = Input.GetKeyDown(KeyCode.Space);
        bool inputGrab = Input.GetMouseButtonDown(1);

        states.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        states.verticalSpeed = Input.GetAxisRaw("Vertical");

        states.moving = Mathf.Abs(states.horizontalSpeed) > 0 || Mathf.Abs(states.verticalSpeed) > 0;
        // states.moveSpeed = Mathf.Sqrt(states.horizontalSpeed * states.horizontalSpeed + states.verticalSpeed * states.verticalSpeed);

        states.lookAtDirection = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        states.movementDirection = new Vector2(states.horizontalSpeed, states.verticalSpeed).normalized;
        states.flashDirection = states.movementDirection.magnitude > 0.1f ? states.movementDirection : states.flashDirection;

        states.meleeAttacking = inputAttack || states.meleeAttacking;
        states.flash = inputFlash && states.moving && Time.time > states.lastFlashTime + flashGap || states.flash;

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
            gameObject.transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * states.moveSpeed, states.verticalSpeed * Time.deltaTime * states.moveSpeed, 0);
        }
        else if (states.flash && false)
        {
            // gameObject.transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, states.verticalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, 0);
            // gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, flashDestiny, Time.deltaTime * flashSpeed);
        }

        if (!states.flash)
        {
            transform.up = states.lookAtDirection;
        }
        else
        {
            transform.up = states.movementDirection;
        }

        // flash
        if (states.flash)
        {
            states.lastFlashTime = Time.time;
        }
    }

    public override void UpdateBehaviour()
    {
        if (states.meleeAttacking)
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
        return;
        if (states.weaving && states.invincible)
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
        if (states.weaving && states.invincible)
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
        // states.freezeMovement = true;
        states.moveSpeed = walkSpeed / 3f;
        if (states.weaving)
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
        // states.freezeMovement = false;
        states.moveSpeed = walkSpeed;
        if (states.weaving)
        {
            states.invincible = false;
        }
    }

    public void StartAttack()
    {
        states.weaving = false;
        states.meleeAttacking = true;
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

    public void StopAttack()
    {
        states.meleeAttacking = false;
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
        int prevIndex = states.comboStep;
        int randomIndex = Random.Range(0, 4);
        // float jabProb = Random.Range(0f, 1f);
        // if (jabProb > 0.4)
        // {
        //     randomIndex = 0;
        // }
        if (randomIndex != prevIndex || randomIndex == 0)
        {
            states.comboStep = randomIndex;
        }
        else
        {
            states.comboStep = (randomIndex + 1) % 4;
        }
        states.idleBoxing = true;
    }

    public void PrepareFlash()
    {
        GameManager.instance.effectDisplayController.PlayGroundDustDirected(transform.position, transform.rotation);
    }

    public void StartFlash()
    {
        float flashDistanceMin = VisionUtil.UnblockedDistance(gameObject, states.flashDirection, flashDistance, new List<string> { "Player", "Weapon" });
        flashDestiny = gameObject.transform.position + (Vector3)states.flashDirection * flashDistanceMin;
        gameObject.transform.position += (Vector3)states.flashDirection * flashDistanceMin;
        // gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, flashDestiny, Time.deltaTime * flashSpeed);
        GameManager.instance.effectDisplayController.PlayGroundDustCircle(transform.position);
        states.flash = false;
    }
}
