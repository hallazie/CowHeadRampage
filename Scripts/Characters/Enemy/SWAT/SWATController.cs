using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWATStates : EnemyStates
{
    public int ammoRemains = 12;

    public float health = 20f;
    public float lastShootTime = 0f;

    public Vector3 distance = Vector3.zero;
    public Vector3 target = Vector3.zero;
    public Vector3 headingDirection = Vector3.zero;
    public Vector3 lastSeenPosition = Vector3.zero;
    public Vector3 lastPatrolPosition = Vector3.zero;

    public bool patrolOverIdle = false;
    public bool allowAttack = false;
    public bool shooting = false;
    public bool reloading = false;
    public bool patrolling = false;

}

public class SWATController : EnemyController
{

    new public SWATStates states;

    public SpriteRenderer gunfireRenderer;
    public SpriteRenderer gunRenderer;
    public GameObject bulletPrefab;
    public Animator animator;
    public Sprite[] deadSpriteList = new Sprite[0];
    public Sprite gunSprite;
    public PatrolRoutine patrolRoutine = null;

    public float visionRange = 20f;
    public float moveSpeed = 10f;
    public float shootGap = 0.1f;
    public float bulletDamage = 10f;
    public float shootJitter = 0.2f;
    public float minNavDestinationDist = 0.1f;

    public int bleedAmount = 25;

    private DropdownShadow shadowCaster;
    private List<Vector2> patrolNavList = null;

    public void Awake()
    {
        states = new SWATStates();
        animationController = new SWATAnimationController(this);
        animator = GetComponent<Animator>();
        shadowCaster = GetComponent<DropdownShadow>();
        gunfireRenderer = transform.Find("GunfireEffect").GetComponent<SpriteRenderer>();
        gunRenderer = transform.Find("Gun").GetComponent<SpriteRenderer>();
        Init(states);

        if (patrolRoutine != null)
        {
            patrolNavList = patrolRoutine.GetPatrolList();
            states.patrolTargetQueue = new Queue<Vector3>();
            foreach (Vector2 spot in patrolNavList)
            {
                states.patrolTargetQueue.Enqueue(spot);
            }
            if (patrolNavList.Count > 0)
            {
                states.patrolOverIdle = true;
            }
        }
    }


    void Start()
    {
        gunfireRenderer.enabled = false;
        gunRenderer.enabled = false;
    }

    void Update()
    {
        if (!states.alive)
        {
            return;
        }
        UpdateStates();
        UpdateAnimation();
        UpdateMovement();
        UpdateBehaviour();
    }

    public override void UpdateStates()
    {
        float playerDistance = ((Vector2)(transform.position - GameManager.instance.player.transform.position)).magnitude;
        if (playerDistance > visionRange)
        {
            states.playerVisible = false;
        }
        else
        {
            states.playerVisible = VisionUtil.CanSeeEachOther(transform.gameObject, GameManager.instance.player.transform.gameObject, new List<string> { "Enemy", "Weapon", "ThrowableItem", "Bullet", "Untagged", "UnblockingEnv" });
        }
        if (states.playerVisible)
        {
            states.shooting = true;
            states.hostilityLevel = 2;
            states.hostileNavQueue = null;
            states.navNextPositon = Vector3.zero;
            CollideEachOther(true, "Enemy");
            if (Time.time >= states.lastShootTime + shootGap)
            {
                if (states.ammoRemains == 0)
                {
                    gunfireRenderer.enabled = false;
                    states.reloading = true;
                }
                else
                {
                    gunfireRenderer.enabled = true;
                    Shoot();
                    states.ammoRemains -= 1;
                    states.lastShootTime = Time.time;
                    GameManager.instance.effectDisplayController.DrawBulletShell(transform.position);
                }
            }
            states.lastSeenPosition = GameManager.instance.player.transform.position;
        }
        else
        {
            states.shooting = false;
            gunfireRenderer.enabled = false;
            states.hostileNavQueue = null;
            if (states.hostilityLevel > 1 && states.lastSeenPosition != Vector3.zero)
            {
                states.hostileNavQueue = GameManager.instance.gridNavController.FindVertexQueue(transform.position, states.lastSeenPosition);
            }
        }
    }

    public override void UpdateMovement()
    {
        if (states.playerVisible)
        {
            Vector3 target = GameManager.instance.player.transform.position;
            Vector3 direction = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0);
            transform.up = direction.normalized;
            states.moveSpeed = 0;
        }
        if (states.hostileNavQueue != null)
        {
            base.TrackPlayer(minNavDestinationDist, moveSpeed);
        }
        if (states.hostileNavQueue == null && states.patrolOverIdle)
        {
            base.FindPatrolTarget(patrolNavList);
            base.Patrol(minNavDestinationDist, moveSpeed);
        }
    }

    public override void UpdateAnimation()
    {
        animationController.UpdateAnimationParameter();
    }

    public override void UpdateBehaviour()
    {

    }

    // ===================== Behaviour Events =====================

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, gunfireRenderer.transform.position, Quaternion.identity);
        bullet.transform.parent = GameManager.instance.transform;
        bullet.name = "Bullet";
        bullet.tag = "Bullet";
        MessageInstantiateBullet bulletMessage = new MessageInstantiateBullet("Player", gameObject.tag, GameManager.instance.player.transform.position, gunfireRenderer.transform.position, bulletDamage, jitter: shootJitter);
        bullet.SendMessage("Init", bulletMessage);
    }

    private int GetDeadSpriteIndex()
    {
        /*
         TODO: select dead sprite (or animation by condition)
         */
        int index = Random.Range(0, deadSpriteList.Length);
        return index;
    }

    public void Dead()
    {
        animator.enabled = false;
        gunfireRenderer.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSpriteList[GetDeadSpriteIndex()];
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 100;
        transform.up = transform.up * -1;
        
        foreach (BoxCollider2D boxCollider2D in gameObject.GetComponentsInChildren<BoxCollider2D>())
        {
            boxCollider2D.enabled = false;
        }
        shadowCaster.shadowSize = 0.05f;
        shadowCaster.OffsetShadow();

        Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        Vector3 randomRotate = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0);
        gunRenderer.enabled = true;
        gunRenderer.sortingLayerName = "GroundStuff";
        gunRenderer.sortingOrder = 100;
        gunRenderer.transform.up = randomRotate;
        gunRenderer.transform.position += randomOffset;
        gunRenderer.sprite = gunSprite;

        GameManager.instance.effectDisplayController.PlayBloodFlow(transform.position, transform.rotation);
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.BroadcastEnemyHostility(range: 100f, requiredVisible: false);

        states.health -= message.damageAmount;
        if (states.health <= 0)
        {
            states.health = 0;
            // animationController.PlayAnimation(WolfHeadAnimationStates.Die, overwrite: true);
            states.alive = false;
            Dead();
        }
    }

    public override void DamagedEffect(MessageAttackEffect message)
    {
        base.DamagedEffect(message);
        if (!states.alive)
        {
            Vector3 targetPosition = transform.position + message.damageDirection.normalized * 5f;
            transform.position = targetPosition;
        }
        GameManager.instance.effectDisplayController.DrawBloodSpread(transform.position, transform.rotation, bleedAmount, 3f);

    }

    // =================================== Animation Events ===================================

    public void StartReload()
    {
        states.reloading = true;
    }

    public void StopReload()
    {
        states.reloading = false;
        states.ammoRemains = 12;
    }

}
