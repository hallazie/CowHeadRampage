using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CowHeadMovement : MonoBehaviour
{

    private CowHeadStatus status = new CowHeadStatus();

    public Animator animator;
    public BoxCollider2D characterBoxCollider;
    public BoxCollider2D weaponBoxCollider;
    public ContactFilter2D weaponBoxFilter;

    public float Speed = 5f;

    private bool enableDamage;
    private Collider2D[] weaponHits = new Collider2D[10]; 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        status.horizontalSpeed = Input.GetAxisRaw("Horizontal");
        status.verticalSpeed = Input.GetAxisRaw("Vertical");
        status.speed = Mathf.Sqrt(status.horizontalSpeed * status.horizontalSpeed + status.verticalSpeed * status.verticalSpeed);
        status.attack = Input.GetMouseButtonDown(0) || status.attack;
        status.lookAtPosition = ((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2) transform.position).normalized;

        UpdateAnimationByCHStatus();

        if (enableDamage)
        {
            weaponBoxCollider.OverlapCollider(weaponBoxFilter, weaponHits);
            for (int i = 0; i < weaponHits.Length; i++)
            {
                if (weaponHits[i] != null && weaponHits[i].tag == "Enemy")
                    print("Colliding with " + weaponHits[i].name);
            }
        }

    }

    private void FixedUpdate()
    {
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

    public void StopAttack()
    {
        status.attack = false;
    }

    public void DamageSwitch()
    {
        if (status.attack && !enableDamage)
        {
            enableDamage = true;
        }
        else
        {
            enableDamage = false;
        }
    }

}
