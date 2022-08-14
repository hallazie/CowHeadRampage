using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfHeadState
{
    public float health = 0f;
    public float moveSpeed = 0f;

    public Vector3 distance = Vector3.zero;
    public Vector3 target = Vector3.zero;

    public int hostilityLevel = 1;                   // 0: friend, 1: neutral, 2: hostile, 3: enemy

    public bool alive = true;
    public bool playerVisible = false;
    public bool allowAttack = false;
    public bool allowAim = false;                    // 是否开始进入瞄准姿态
    public bool allowShoot = false;                  // 是否可进行射击
    public bool lostTrack = false;

    public string attackMode = "None";
}

public class WolfHeadController : AttackablePawn
{

    public GameObject cowHead;
    public GameObject interactionManager;
    public GameObject bulletPrefab;

    public Animator animator;
    public MeleeWeapon weapon;
    public Sprite deadSprite;
    public List<Sprite> deadSpriteList;
    public WolfHeadAnimationController animationController;
    public WolfHeadState states;
    public Rigidbody2D rgdbody;

    public string characterType = "WolfHead";
    public float runSpeed = 1f;
    public float sprintFactor = 1.5f;
    public float visionRange = 16f;
    public float sprintRange = 8f;
    public float fistAttackRange = 0.5f;
    public float minContactRange = 0.1f;
    public bool drawGizmos = false;
    public float interactRange = 1f;
    public float gizmosRange = 1f;
    public float hostileDuration = 5f;

    public float shootProbability = 0.5f;
    public float maxShootRange = 3f;
    public float aimingMinTime = 0.5f;
    public float aimingMaxTime = 1f;

    public int attackDamage = 10;
    public float pistolDamage = 5f;
    public float maxHealth = 20;
    public float attackGap = 0.8f;

    private Vector3 originalPosition;
    private Queue<Vector3> hostileNavQueue = null;
    private Queue<Vector3> patrolNavQueue = null;
    private Vector3 nextNavPosition;
    private float nextAttackTime = 0f;
    private float lastSawPlayerTime = 0f;
    private float lastShootTime = 0f;
    private float currentAimingTime = 0f;

    // weapon attrs
    public Color fistDamageColor = Color.red;

    private void Awake()
    {
        weapon = GetComponentInChildren<MeleeWeapon>();
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
        // states.hostilityLevel = 3;

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
            states.attackMode = "None";
        }
        else
        {
            // 目标可见
            lastSawPlayerTime = Time.time;
            float shootProb = Random.Range(0f, 1f);
            if (states.hostilityLevel > 1 && states.attackMode == "None")
            {
                if (shootProb <= shootProbability && states.distance.magnitude <= maxShootRange)
                {
                    states.allowAim = true;
                    states.attackMode = "Shoot";
                    lastShootTime = Time.time;
                    currentAimingTime = Random.Range(aimingMinTime, aimingMaxTime);
                }
                else
                {
                    states.allowAim = false;
                    states.attackMode = "Melee";
                }

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

            if (states.playerVisible)
            {
                // TODO 判断destination是否改变，若改变再重新进行ASTAR算法，减少消耗。
                hostileNavQueue = GameManager.instance.navGridManager.FindVertexQueue(transform.position, cowHead.transform.position);
                //if (hostileNavQueue != null)
                //{
                //    hostileNavList = new List<Vector3>();
                //    while (hostileNavQueue.Count > 0)
                //    {
                //        hostileNavList.Add(hostileNavQueue.Dequeue());
                //    }
                //    hostileNavQueue.Clear();
                //    foreach (Vector3 pos in hostileNavList)
                //    {
                //        hostileNavQueue.Enqueue(pos);
                //    }
                //}
            }

            // 敌对模式
            if (states.allowAim && states.playerVisible)
            {
                transform.up = states.target.normalized;
            }
            else if (states.distance.magnitude < visionRange && states.distance.magnitude > fistAttackRange)
            {
                // can see player, go chase
                if (states.distance.magnitude > sprintRange)
                {
                    states.target *= sprintFactor;
                }

                // movement
                if (hostileNavQueue != null)
                {
                    Vector2 navMovePosition = (Vector2)(transform.position - nextNavPosition);
                    if (nextNavPosition == Vector3.zero || navMovePosition.magnitude <= 0.01f && hostileNavQueue != null && hostileNavQueue.Count > 0)
                    {
                        nextNavPosition = hostileNavQueue.Dequeue();
                    }
                    else if (navMovePosition.magnitude <= 0.01f && hostileNavQueue != null && hostileNavQueue.Count == 0)
                    {
                        states.lostTrack = true;
                        states.moveSpeed = 0f;
                    }
                    else
                    {
                        states.lostTrack = false;
                        Vector2 heading = -1 * navMovePosition.normalized;
                        Vector3 target = new Vector3(heading.x * runSpeed * Time.deltaTime, heading.y * runSpeed * Time.deltaTime, 0);
                        if (navMovePosition.magnitude > 0.001f)
                        {
                            // print("heading: " + heading + ", target: " + target + " with mag: " + target.magnitude + " and sqr mag: " + target.sqrMagnitude);
                            transform.up = target.normalized;
                            transform.position += target;
                            states.moveSpeed = target.magnitude;
                        }
                        else
                        {
                            states.moveSpeed = 0f;
                        }
                    }
                }

                //// no use nav
                //transform.up = states.target.normalized;
                //transform.position += states.target;
            }
            else if (states.distance.magnitude <= fistAttackRange)
            {
                hostileNavQueue = null;
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
                if (states.distance.magnitude > minContactRange)
                {
                    transform.up = states.target.normalized;
                    transform.position += states.target;
                }
            }
            else
            {
                hostileNavQueue = null;
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
            if (hit.transform.gameObject == null)
            {
                continue;
            }
            if (hit.transform.parent != null && (hit.transform.parent.gameObject.layer == GameManager.instance.layerDict["Player"] || hit.transform.parent.gameObject.layer == GameManager.instance.layerDict["Enemy"]))
            {
                continue;
            }
            if (hit.transform.gameObject.tag == "Furniture")
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

        //if (hostileNavList != null)
        //{
        //    // print("draw nav gizmos with queue size: " + hostileNavList.Count);
        //    Vector3 prevGizmosNav = Vector3.zero;
        //    for (int i = 0; i < hostileNavList.Count; i++)
        //    {
        //        Vector3 nextGizmosNav = hostileNavList[i];
        //        if (prevGizmosNav == Vector3.zero)
        //        {
        //            prevGizmosNav = nextGizmosNav;
        //            continue;
        //        }
        //        else
        //        {
        //            Gizmos.DrawLine((Vector2)prevGizmosNav, (Vector2)nextGizmosNav);
        //        }
        //        prevGizmosNav = nextGizmosNav;
        //    }
        //}
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
        states.attackMode = "None";
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
        states.attackMode = "None";
        weapon.enableDamage = false;
        weapon.singleRoundHit.Clear();
        gameObject.transform.localScale = Vector3.one;
    }

    public override void CauseDamage()
    {
        weapon.OnAttack("Player", ignoringTags: new List<string> { "Bullet", "Weapon", "Player", "Enemy" });
    }

    public override void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.BroadcastEnemyHostility();
        // FloatingTextManager.instance.ShowBasic("-" + message.damageAmount.ToString(), Color.blue, gameObject.transform.position, Vector3.up * 64, duration: 2f, fontSize: 32);

        states.health -= message.damageAmount;
        if (states.health <= 0)
        {
            states.health = 0;
            // animationController.PlayAnimation(WolfHeadAnimationStates.Die, overwrite: true);
            states.alive = false;
            Dead();
        }
    }

    public override void AttackEffect(MessageAttackEffect message)
    {

    }

    public override void DamagedEffect(MessageAttackEffect message)
    {
        Vector3 direction = (message.origin - message.target);
        direction.z = 0;
        direction = direction.normalized;
        Vector3 position = new Vector3(message.target.x - direction.x * 0.3f, message.target.y - direction.y * 0.3f, 0);
        Vector3 contactProximatePosition = (message.target + message.origin) / 2f;
        GameManager.instance.effectDisplayController.DrawBloodSpread(position, -1 * direction);
        GameManager.instance.effectDisplayController.PlayBlastEffect(contactProximatePosition);
        GameManager.instance.ShakeCamera();
    }

    public override void Dead()
    {
        animator.enabled = false;
        int deadSpriteIndex = Random.Range(0, deadSpriteList.Count);
        switch (deadSpriteIndex)
        {
            case 0:
                break;
            case 1:
                transform.up = states.target.normalized * -1;
                break;
        }
        // rgdbody.AddForce((transform.position - cowHead.transform.position).normalized * 5f);
        // StartCoroutine(SlideOnDirection(transform.position - cowHead.transform.position, 5f));
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSpriteList[deadSpriteIndex];
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }

        hostileNavQueue = null;
        nextNavPosition = Vector3.zero;
    }

    public IEnumerator SlideOnDirection(Vector3 direction, float distance)
    {
        Vector3 destination = direction * distance;
        while ((destination - transform.position).magnitude < 0.001f)
        {
            // transform.position += Vector3.Lerp(transform.position, destination, 0.1f);
            transform.position += direction * 0.1f;
            yield return null;
        }

    }
}
