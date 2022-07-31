using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigHeadState
{
    public float health = 0f;
    public float moveSpeed = 0f;
    public Vector3 distance = Vector3.zero;
    public Vector3 target = Vector3.zero;

    public bool playerVisible = false;
    public bool alive = true;
    public int hostilityLevel = 1;                   // 0: friend, 1: neutral, 2: hostile, 3: enemy
}

public class PigHeadController : AttackablePawn
{

    public Rigidbody2D rgdbody;
    public Animator animator;

    public PigHeadState states;
    public PigHeadAnimationController animationController;

    private void Awake()
    {
        animationController = new PigHeadAnimationController(this, animator);
        states = new PigHeadState();
    }

    // Start is called before the first frame update
    void Start()
    {
        rgdbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!states.alive)
            return;

        // UpdateStates();
        // UpdateMovement();
        animationController.UpdateAnimationParameter();
        // UpdateInteraction();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == GameManager.instance.layerDict["Player"])
        {
            rgdbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == GameManager.instance.layerDict["Player"])
        {
            rgdbody.constraints = RigidbodyConstraints2D.None;
        }
    }

    public void StopAnimation()
    {

    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public override void AttackEffect(MessageAttackEffect message)
    {
        
    }

    public override void DamagedEffect(MessageAttackEffect message)
    {
        Vector3 direction = (message.origin - message.target);
        direction.z = 0;
        direction = direction.normalized;
        Vector3 position = new Vector3(message.target.x - direction.x, message.target.y - direction.y, 0);
        Vector3 contactProximatePosition = (message.target + message.origin) / 2f;
        GameManager.instance.effectDisplayController.DrawBloodSpread(position, -1 * direction);
        GameManager.instance.effectDisplayController.PlayBlastEffect(contactProximatePosition);
        GameManager.instance.ShakeCamera();
    }

    public override void CauseDamage()
    {
        
    }

    public override void Dead()
    {
        
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
    {
        
    }

    public override void StartAttack()
    {
        
    }

    public override void StopAttack()
    {
        
    }

}
