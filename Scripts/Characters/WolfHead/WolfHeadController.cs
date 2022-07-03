using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfHeadController : AttackablePawn
{

    public GameObject cowHead;
    public Animator animator;
    public Weapon weapon;

    public float runSpeed = 1f;
    public float visionRange = 5f;
    public float fistAttackRange = 4.5f;

    public int attackDamage = 4;
    public int health = 20;

    private Vector3 originalPosition;
    private bool collideWithPlayer = false;

    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.Init(attackDamage: attackDamage);
        cowHead = GameObject.Find("CowHead");
        originalPosition = gameObject.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = cowHead.transform.position - gameObject.transform.position;
        Vector3 target = new Vector3(distance.normalized.x * runSpeed * Time.deltaTime, distance.normalized.y * runSpeed * Time.deltaTime, 0);
        if (IsVisible() && distance.magnitude < visionRange && distance.magnitude > fistAttackRange)
        {
            transform.up = target.normalized;
            transform.position += target;            
            animator.SetFloat("Speed", 1f);
            animator.SetBool("InFistAttackRange", false);
        }
        else if (distance.magnitude <= fistAttackRange)
        {
            transform.up = target.normalized;
            animator.SetFloat("Speed", 0);
            animator.SetBool("InFistAttackRange", true);
            if (!collideWithPlayer)
            {
                transform.position += target;
            }
            CauseDamage();
        }
        else
        {
            animator.SetFloat("Speed", 0);
            animator.SetBool("InFistAttackRange", false);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collideWithPlayer = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collideWithPlayer = false;
        }
    }

    private bool IsVisible()
    {
        Vector3 direction = cowHead.transform.position - transform.position;
        direction.z = 0;

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(
            origin: new Vector2(transform.position.x, transform.position.y),
            direction: direction.normalized,
            distance: direction.magnitude
        );
        if (hits.Length == 0)
        {
            return false;
        }
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.gameObject.tag != "Player" && hit.transform.gameObject.tag != "Enemy")
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        // if (cowHead != null)
        //    Gizmos.DrawLine(transform.position, cowHead.transform.position);
    }

    public void AttackEffect()
    {

    }


    // ---------------------------- OVERRIDE ATTACKABLE PAWN ----------------------------

    public override void StartAttack()
    {
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

    public override void StopAttack()
    {
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
    }

    public override void CauseDamage()
    {
        weapon.OnAttack("Player");
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
    {
        health -= message.damageAmount;
        if (health < 0)
        {
            health = 0;
        }
    }

    public override void AttackEffect(MessageAttackEffect message)
    {
        
    }
}
