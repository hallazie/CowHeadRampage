using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CowHeadController : AttackablePawn
{

    private CowHeadStatus status = new CowHeadStatus();

    public BloodSpreadController bloodSpreadController;
    public Animator animator;
    public Weapon weapon;
    public Sprite deadSprite;
    public float Speed = 5f;
    public bool alive;

    public int attackDamage = 10;
    public int health = 100;
    public int maxHealth;

    public delegate void AttackDelegate();
    public AttackDelegate attackDelegate;


    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.Init(attackDamage: attackDamage);
        alive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;   
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
            return;
        status.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        status.verticalSpeed = Input.GetAxisRaw("Vertical");
        status.speed = Mathf.Sqrt(status.horizontalSpeed * status.horizontalSpeed + status.verticalSpeed * status.verticalSpeed);
        status.attack = Input.GetMouseButtonDown(0) || status.attack;
        status.lookAtPosition = ((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2) transform.position).normalized;

        UpdateAnimationByCHStatus();

        if (status.attack)
        {
            CauseDamage();
        }

    }

    private void FixedUpdate()
    {
        if (!alive)
            return;
        transform.position += new Vector3(status.horizontalSpeed * Time.deltaTime * Speed, status.verticalSpeed * Time.deltaTime * Speed, 0);
        transform.up = status.lookAtPosition;
    }

    private void UpdateAnimationByCHStatus()
    {
        /*
         使用script替代animation状态机，获得更好的控制
         */
        status.ResolveStatusConflict();
        animator.SetFloat("Speed", status.speed);
        animator.SetBool("Attack", status.attack);
    }

    public void Respawn()
    {
        /*
         复活，类似MH中按R复活
         */
        alive = true;
        health = maxHealth;
        animator.enabled = true;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = true;
        }
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Attack", false);
        animator.SetBool("Dead", false);
    }

    // ---------------------------- OVERRIDE ATTACKABLE PAWN ----------------------------

    public override void StopAttack()
    {
        status.attack = false;
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
        health -= message.damageAmount;
        if (health <= 0)
        {
            health = 0;
            animator.SetBool("Dead", true);
            alive = false;
            GameManager.instance.playerAlive = false;
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
