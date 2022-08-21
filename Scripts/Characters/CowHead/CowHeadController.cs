using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowHeadState
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


public class CowHeadController : AttackablePawn
{

    public CowHeadAnimationController animationController;
    public CowHeadState states;

    public Animator animator;
    public MeleeWeapon weapon;
    public Sprite deadSprite;
    public Rigidbody2D rgdbody;
    public BoxCollider2D boxCollider;
    public BoxCollider2D damageCollider;

    public string characterType = "CowHead";
    public float runSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public int attackDamage = 10;
    public float attackGap = 0.5f;
    public float idleBoxingGap = 2f;
    public int maxHealth = 100;

    // weapon attrs
    public Color knifeDamageColor = Color.blue;


    private void Awake()
    {
        states = new CowHeadState();
        weapon = GetComponentInChildren<MeleeWeapon>();
        weapon.Init(attackDamage: attackDamage, fontColor: knifeDamageColor);

        rgdbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        animationController = new CowHeadAnimationController(this, animator);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (states == null)
        {
            states = new CowHeadState();
        }
        states.alive = true;
        states.health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!states.alive || states.occupied)
            return;
        UpdateStates();
        animationController.UpdateAnimationParameter();
        UpdateMovement();
        if (states.attack)
        {
            CauseDamage();
        }
    }

    private void FixedUpdate()
    {

    }

    private void UpdateStates()
    {
        bool inputAttack = Input.GetMouseButton(0) || Input.GetMouseButtonDown(0);

        states.horizontalSpeed = Input.GetAxisRaw("Horizontal");    
        states.verticalSpeed = Input.GetAxisRaw("Vertical");
        states.moveSpeed = Mathf.Sqrt(states.horizontalSpeed * states.horizontalSpeed + states.verticalSpeed * states.verticalSpeed);
        //if (!states.attack && inputAttack)
        //{
        //    if (Time.time - states.lastAttackTime < 0.1f)
        //    {
        //        states.comboStep = 2;
        //    }
        //    states.lastAttackTime = Time.time;
        //}
        states.attack = inputAttack || states.attack;
        states.lookAtDirection = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        states.movementDirection = new Vector2(states.horizontalSpeed, states.verticalSpeed).normalized;

        float angle = Vector2.Angle(states.lookAtDirection, states.movementDirection);
        if (angle >= 90)
        {
            states.sprint = false;
        }

        // weave logics
        states.weave = Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space) || states.weave;
        if (states.weave && states.weaveType == null)
        {
            float randomIndex = Random.Range(0f, 1f);
            if (randomIndex > 0.5f)
            {
                states.weaveType = "left";
            }
            else
            {
                states.weaveType = "right";
            }
        }
        if (Time.time - states.lastBoxingTime <= idleBoxingGap)
        {
            states.idleBoxing = true;
        }
        else
        {
            states.idleBoxing = false;
        }
    }

    private void UpdateMovement()
    {
        /*
         update movements in fixed update
         */

        if (states.occupied || states.freezeMovement)
            return;
        if (!states.sprint)
        {
            // Vector3 nextPosition = new Vector3(transform.position.x + states.horizontalSpeed * Time.deltaTime * runSpeed, transform.position.y + states.verticalSpeed * Time.deltaTime * runSpeed, 0);
            // transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * runSpeed);
            gameObject.transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed, states.verticalSpeed * Time.deltaTime * runSpeed, 0);
        }
        else
        {
            gameObject.transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, states.verticalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, 0);
        }
        transform.up = states.lookAtDirection; 
        // float rotZ = Mathf.Atan2(states.lookAtDirection.y, states.lookAtDirection.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
        // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, rotZ - 90), 1f);
    }

    public void StartConversation()
    {
        states.occupied = true;
    }

    public void StopConversation()
    {
        states.occupied = false;
    }

    public void StopAnimation()
    {
        UnfreezeMovement();
        animationController.StopAnimation();
    }

    public void Respawn()
    {
        /*
         复活，类似HM中按R复活
         */
        states.alive = true;
        states.health = maxHealth;
        animator.enabled = true;
        GameManager.instance.playerAlive = true;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = true;
        }

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

    public void StartWeave()
    {
        states.attack = false;
    }

    public void StopWeave()
    {
        states.weave = false;
        states.weaveType = null;
        states.idleBoxing = true;
        states.lastBoxingTime = Time.time;
    }

    public void EnterCutscene()
    {
        boxCollider.enabled = false;
        damageCollider.enabled = false;
    }

    public void ExitCutscene()
    {
        boxCollider.enabled = true;
        damageCollider.enabled = true;
    }


    // ---------------------------- OVERRIDE ATTACKABLE PAWN ----------------------------

    public override void StopAttack()
    {
        states.attack = false;
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
        float randomIndex = Random.Range(0f, 1f);
        if (randomIndex > 0.5)
        {
            states.comboStep = 1;
        }else
        {
            states.comboStep = 0;
        }
        states.idleBoxing = true;
        states.lastBoxingTime = Time.time;
    }

    public override void StartAttack()
    {
        states.weave = false;

        states.attack = true;
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

    public override void CauseDamage()
    {
        weapon.OnAttack("Enemy", showAttackEffect: true, ignoringTags: new List<string> { "Bullet", "Weapon", "Player", "Enemy" });
        weapon.OnAttack("Bullet", sendDamagedEffect: false, showDamage: false, visualConditional: false);
        weapon.OnAttack("InteractableEnvironment", sendDamage: true, sendDamagedEffect: true, showDamage: false, showAttackEffect: true, visualConditional: false);
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
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
            animationController.PlayAnimation(CowHeadAnimationStates.Die, overwrite: true);
        }
        // FloatingTextManager.instance.ShowBasic("health: " + health.ToString(), Color.yellow, transform.position, Vector3.up * 32, duration: 5f);
    }

    public override void AttackEffect(MessageAttackEffect message)
    {
        print("hitting " + weapon.singleRoundHit.Count + "items");
        // GameManager.instance.SoundController.PlaySoundMultipleTimes(SoundNames.CowHeadPunch, weapon.singleRoundHit.Count, 0.1f);
        GameManager.instance.SoundController.PlaySound(SoundNames.CowHeadPunch);
    }

    public override void DamagedEffect(MessageAttackEffect message)
    {
        if (states.weave && states.invincible)
        {
            return;
        }
    }

    public override void Dead()
    {
        states.alive = false;
        animator.enabled = false;
        GameManager.instance.BroadcastEnemyHostility(1, 1000, false);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
    }
}
