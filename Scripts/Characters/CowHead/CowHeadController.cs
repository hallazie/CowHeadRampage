using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowHeadState
{

    public bool alive;
    public bool occupied;
    public bool freezeMovement;
    public bool attack;
    public bool sprint;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float moveSpeed;
    public float health;
    public float lastAttackTime;

    public Vector2 lookAtDirection;
    public Vector2 movementDirection;

    public int comboStep;

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

    public string characterType = "CowHead";
    public float runSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public int attackDamage = 10;
    public float attackGap = 0.5f;
    public int maxHealth = 100;

    // weapon attrs
    public Color knifeDamageColor = Color.blue;


    private void Awake()
    {
        weapon = GetComponentInChildren<MeleeWeapon>();
        weapon.Init(attackDamage: attackDamage, fontColor: knifeDamageColor);
        states = new CowHeadState();

        rgdbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        animationController = new CowHeadAnimationController(this, animator);
    }

    // Start is called before the first frame update
    void Start()
    {
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
    }

    private void FixedUpdate()
    {
        if (!states.alive || states.occupied)
            return;
        UpdateMovement();
        if (states.attack)
        {
            CauseDamage();
        }
    }

    private void UpdateStates()
    {
        states.sprint = Input.GetKey(KeyCode.LeftShift);
        states.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        states.verticalSpeed = Input.GetAxisRaw("Vertical");
        states.moveSpeed = Mathf.Sqrt(states.horizontalSpeed * states.horizontalSpeed + states.verticalSpeed * states.verticalSpeed);
        states.attack = Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || states.attack;
        states.lookAtDirection = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        states.movementDirection = new Vector2(states.horizontalSpeed, states.verticalSpeed).normalized;

        float angle = Vector2.Angle(states.lookAtDirection, states.movementDirection);
        if (angle >= 90)
        {
            states.sprint = false;
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
            transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed, states.verticalSpeed * Time.deltaTime * runSpeed, 0);
        }
        else
        {
            transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, states.verticalSpeed * Time.deltaTime * runSpeed * sprintMultiplier, 0);
        }
        transform.up = states.lookAtDirection; 

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
    }

    public void UnfreezeMovement()
    {
        states.freezeMovement = false;
    }

    // ---------------------------- OVERRIDE ATTACKABLE PAWN ----------------------------

    public override void StopAttack()
    {
        states.attack = false;
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
        if (states.comboStep == 0)
        {
            states.comboStep = 1;
        }else if (states.comboStep == 1)
        {
            states.comboStep = 0;
        }
    }

    public override void StartAttack()
    {
        states.attack = true;
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

    public override void CauseDamage()
    {
        weapon.OnAttack("Enemy", ignoringTags: new List<string> { "Bullet", "Weapon", "Player", "Enemy" });
        weapon.OnAttack("Bullet", sendDamagedEffect: false, showDamage: false, visualConditional: false);
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
    {
        states.health -= message.damageAmount;
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

    }

    public override void DamagedEffect(MessageAttackEffect message)
    {

    }

    public override void Dead()
    {
        animator.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
    }
}
