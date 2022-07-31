using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerPawnController : AttackablePawn
{
    // Start is called before the first frame update
    public Animator upperBodyAnimator;
    public Animator lowerBodyAnimator;

    public float horizontalSpeed;
    public float verticalSpeed;
    public float moveSpeed;
    public bool attack = false;
    public Vector2 lookAtPosition;

    public float speed = 5f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalSpeed = Input.GetAxisRaw("Horizontal");
        verticalSpeed = Input.GetAxisRaw("Vertical");
        moveSpeed = Mathf.Sqrt(horizontalSpeed * horizontalSpeed + verticalSpeed * verticalSpeed);
        attack = Input.GetMouseButtonDown(0) || attack;
        lookAtPosition = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;

        upperBodyAnimator.SetFloat("Speed", moveSpeed);
        upperBodyAnimator.SetBool("Attack", attack);

    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(horizontalSpeed * Time.deltaTime * speed, verticalSpeed * Time.deltaTime * speed, 0);
        transform.up = lookAtPosition;
    }

    public override void StartAttack()
    {
    }

    public override void StopAttack()
    {
        attack = false;
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
    {
        
    }

    public override void CauseDamage()
    {
        
    }

    public override void AttackEffect(MessageAttackEffect message)
    {
        
    }

    public override void Dead()
    {
        
    }

    public override void DamagedEffect(MessageAttackEffect message)
    {

    }
}
