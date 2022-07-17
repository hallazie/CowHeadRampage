using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowHeadState
{

    public bool alive;
    public float health;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float moveSpeed;

    public Vector2 lookAtPosition;

    public bool attack;

}


public class CowHeadController : AttackablePawn
{

    public CowHeadAnimationController animationController;
    public CowHeadState states;

    public BloodSpreadController bloodSpreadController;
    public Animator animator;
    public Weapon weapon;
    public Sprite deadSprite;
    public float runSpeed = 5f;

    public int attackDamage = 10;
    public int maxHealth = 100;

    public delegate void AttackDelegate();
    public AttackDelegate attackDelegate;


    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.Init(attackDamage: attackDamage);
        states = new CowHeadState();
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
        if (!states.alive)
            return;

        UpdateStates();
        UpdateMovement();
        animationController.UpdateAnimationParameter();

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
        transform.position += new Vector3(states.horizontalSpeed * Time.deltaTime * runSpeed, states.verticalSpeed * Time.deltaTime * runSpeed, 0);
        transform.up = states.lookAtPosition;

    }

    private void FixedUpdate()
    {
        if (!states.alive)
            return;
        UpdateMovement();
        if (states.attack)
        {
            CauseDamage();
        }
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

    // ---------------------------- OVERRIDE ATTACKABLE PAWN ----------------------------

    public override void StopAttack()
    {
        states.attack = false;
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
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
        Vector3 direction = (message.target - message.origin).normalized;
        direction.z = 0;
        Vector3 position = new Vector3(message.target.x + direction.x * 5, message.target.y + direction.y * 5, 0);
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
