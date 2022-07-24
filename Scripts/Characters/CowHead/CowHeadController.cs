using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowHeadState
{

    public bool alive;
    public bool occupied;
    public bool freezeMovement;
    public bool attack;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float moveSpeed;
    public float health;
    public float lastAttackTime;

    public Vector2 lookAtPosition;

    public int comboStep;

}


public class CowHeadController : AttackablePawn
{

    public CowHeadAnimationController animationController;
    public CowHeadState states;

    public BloodSpreadController bloodSpreadController;
    public Animator animator;
    public Weapon weapon;
    public Sprite deadSprite;
    public Rigidbody2D rgdbody;

    public string characterType = "CowHead";
    public float runSpeed = 5f;
    public int attackDamage = 10;
    public float attackGap = 0.5f;
    public int maxHealth = 100;

    public delegate void AttackDelegate();
    public AttackDelegate attackDelegate;


    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.Init(attackDamage: attackDamage);
        states = new CowHeadState();

        rgdbody = GetComponent<Rigidbody2D>();

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
        UpdateMovement();
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
        states.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        states.verticalSpeed = Input.GetAxisRaw("Vertical");
        states.moveSpeed = Mathf.Sqrt(states.horizontalSpeed * states.horizontalSpeed + states.verticalSpeed * states.verticalSpeed);
        states.attack = Input.GetMouseButtonDown(0) || states.attack;
        states.lookAtPosition = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;

    }

    private void UpdateMovement()
    {
        /*
         update movements in fixed update
         */

        if (states.occupied || states.freezeMovement)
            return;

        transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed, states.verticalSpeed * Time.deltaTime * runSpeed, 0);
        transform.up = states.lookAtPosition;

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
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

    public override void CauseDamage()
    {
        weapon.OnAttack("Enemy");
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
        Vector3 direction = (message.target - message.origin);
        direction.z = 0;
        direction = direction.normalized;
        Vector3 position = new Vector3(message.target.x + direction.x, message.target.y + direction.y, 0);
        bloodSpreadController.DrawBloodSpread(position, direction);
    }

    public override void Dead()
    {
        animator.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
    }
}
