using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfHeadState
{
    public float health = 0f;
    public float moveSpeed = 0f;
    public Vector3 distance = Vector3.zero;
    public Vector3 target = Vector3.zero;
    public bool playerVisible = false;
    public bool allowAttack = false;
    public bool alive = true;
    public int hostilityLevel = 1;                   // 0: friend, 1: neutral, 2: hostile, 3: enemy
    public bool allowAim = false;                    // 是否开始进入瞄准姿态
    public bool allowShoot = false;                  // 是否可进行射击
    public bool attackMode = false;
}

public class WolfHeadController : AttackablePawn
{

    public GameObject cowHead;
    public GameObject interactionManager;
    public GameObject bulletPrefab;

    public Animator animator;
    public Weapon weapon;
    public Sprite deadSprite;
    public WolfHeadAnimationController animationController;
    public WolfHeadState states;
    public Rigidbody2D rgdbody;

    public string characterType = "WolfHead";
    public float runSpeed = 1f;
    public float sprintFactor = 1.5f;
    public float visionRange = 16f;
    public float sprintRange = 8f;
    public float fistAttackRange = 0.5f;
    public bool drawGizmos = false;
    public float interactRange = 1f;
    public float gizmosRange = 1f;
    public float hostileDuration = 5f;

    public float shootProbability = 0.5f;
    public float aimingMinTime = 0.5f;
    public float aimingMaxTime = 2f;

    public int attackDamage = 10;
    public float pistolDamage = 5f;
    public float maxHealth = 20;
    public float attackGap = 0.8f;

    private Vector3 originalPosition;
    private float nextAttackTime = 0f;
    private float lastSawPlayerTime = 0f;
    private float lastShootTime = 0f;
    private float currentAimingTime = 0f;

    // weapon attrs
    public Color fistDamageColor = Color.red;

    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.Init(attackDamage: attackDamage, fontColor: fistDamageColor);

        cowHead = GameObject.Find("CowHead");
        interactionManager = GameObject.Find("InteractionManager");

        rgdbody = GetComponent<Rigidbody2D>();
        

        animationController = new WolfHeadAnimationController(this, animator);
        states = new WolfHeadState();
        originalPosition = gameObject.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        states.health = maxHealth;
    }

    void Update()
    {
        if (!states.alive)
            return;

        UpdateStates();
        UpdateMovement();
        animationController.UpdateAnimationParameter();
        UpdateInteraction();
    }

    private void UpdateStates()
    {
        states.playerVisible = IsPlayerVisible();
        if (!states.playerVisible)
        {
            // 丢失目标
            if(Time.time - lastSawPlayerTime >= hostileDuration)
            {
                states.hostilityLevel = 1;
            }
            states.allowAim = false;
            states.allowShoot = false;
            states.attackMode = false;
        }
        else
        {
            // 目标可见
            lastSawPlayerTime = Time.time;
            float shootProb = Random.Range(0f, 1f);
            if (!states.attackMode && shootProb <= shootProbability )
            {
                states.allowAim = true;
                states.attackMode = true;
                lastShootTime = Time.time;
                currentAimingTime = Random.Range(aimingMinTime, aimingMaxTime);
            }
            if (states.hostilityLevel > 1)
            {
                states.attackMode = true;
            }
        }
        if (states.allowAim && !states.allowShoot && Time.time - lastShootTime > currentAimingTime)
        {
            states.allowShoot = true;
        }
        states.distance = cowHead.transform.position - gameObject.transform.position;
        states.distance.z = 0f;
        states.target = new Vector3(states.distance.normalized.x * runSpeed * Time.deltaTime, states.distance.normalized.y * runSpeed * Time.deltaTime, 0);
    }

    private void UpdateMovement()
    {
        if (states.hostilityLevel == 3)
        {
            // 敌对模式
            if (states.allowAim)
            {
                transform.up = states.target.normalized;
            }
            else if (states.playerVisible && states.distance.magnitude < visionRange && states.distance.magnitude > fistAttackRange)
            {
                // can see player, go chase
                if (states.distance.magnitude > sprintRange)
                {
                    states.target *= sprintFactor;
                }
                transform.up = states.target.normalized;
                transform.position += states.target;
            }
            else if (states.distance.magnitude <= fistAttackRange)
            {
                // can attack palyer
                if (Time.time > nextAttackTime)
                {
                    // wait amount of time
                    states.allowAttack = true;
                    CauseDamage();
                }
                else
                {
                    states.allowAttack = false;
                }
            }
        }
        else if (states.hostilityLevel == 1)
        {
            // 中立模式
        }

    }
    public void UpdateInteraction()
    {
        if (states.distance.magnitude < interactRange && states.hostilityLevel < 2)
        {
            InteractionSelectorManager.instance.StartIntereaction(this.gameObject);
        }
        else
        {
            InteractionSelectorManager.instance.StopInteraction(this.gameObject);
        }
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



    // ===================================================================

    public void RandomFlipSprite()
    {
        if(Random.Range(0, 10) > 5)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void StartConversation()
    {
        if (states.hostilityLevel < 2)
        {
            DialogueManager.instance.StartDialogue(gameObject);
        }
    }

    public void StopConversation()
    {
        DialogueManager.instance.EndDialogue();
        GameManager.instance.BroadcastEnemyHostility();
        // states.hostilityLevel = 3;
        // StartCoroutine(DelayExecuting(1));
    }


    private bool IsPlayerVisible()
    {
        if (!GameManager.instance.cowHead.states.alive)
        {
            return false;
        }

        Vector3 direction = cowHead.transform.position - transform.position;
        direction.z = 0;

        if (direction.magnitude > visionRange)
            return false;

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(
            origin: new Vector2(transform.position.x, transform.position.y),
            direction: direction.normalized,
            distance: direction.magnitude
        );
        if (hits.Length == 0)
        {
            return true;
        }
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.parent != null && (hit.transform.parent.gameObject.layer == GameManager.instance.layerDict["Player"] || hit.transform.parent.gameObject.layer == GameManager.instance.layerDict["Enemy"]))
            {
                continue;
            }
            if (hit.transform.gameObject.layer != GameManager.instance.layerDict["Player"] && hit.transform.gameObject.layer != GameManager.instance.layerDict["Enemy"])
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;
        if (cowHead != null && states.playerVisible)
           Gizmos.DrawLine(transform.position, cowHead.transform.position);
        Gizmos.DrawWireSphere(transform.position, gizmosRange);

    }

    public void StopAnimation()
    {
        animationController.StopAnimation();
    }

    public void Respawn()
    {
        states.alive = true;
        states.health = maxHealth;
        gameObject.transform.position = originalPosition;
        animator.enabled = transform;
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = true;
        }
    }

    // ---------------------------- OTHERS ----------------------------

    public void StartShoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);
        bullet.name = "Bullet";
        bullet.tag = "Bullet";
        MessageInstantiateBullet bulletMessage = new MessageInstantiateBullet(cowHead.tag, gameObject.tag, cowHead.transform.position, transform.position, pistolDamage);
        bullet.SendMessage("Init", bulletMessage);
    }

    public void StopShoot()
    {
        states.allowAim = true;
        states.allowShoot = false;
        states.attackMode = false;
        lastShootTime = Time.time;
        currentAimingTime = Random.Range(aimingMinTime, aimingMaxTime);
    }

    // ---------------------------- OVERRIDE ATTACKABLE PAWN ----------------------------

    public override void StartAttack()
    {
        weapon.enableDamage = true;
        weapon.singleRoundHit.Clear();
    }

    public override void StopAttack()
    {
        nextAttackTime = Time.time + attackGap;
        states.allowAttack = false;
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
        gameObject.transform.localScale = Vector3.one;
    }

    public override void CauseDamage()
    {
        weapon.OnAttack("Player");
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.BroadcastEnemyHostility();

        states.health -= message.damageAmount;
        if (states.health <= 0)
        {
            states.health = 0;
            animationController.PlayAnimation(WolfHeadAnimationStates.Die, overwrite: true);
            states.alive = false;
        }
    }

    public override void AttackEffect(MessageAttackEffect message)
    {
        
    }

    public override void Dead()
    {
        animator.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
    }
}
