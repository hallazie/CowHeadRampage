using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{

    public bool enableDamage
    {
        set
        {
            enableDamage_ = value;
        }
        get
        {
            return enableDamage_;
        }
    }
    public BoxCollider2D weaponCollider;
    public ContactFilter2D weaponContactFilter;
    public HashSet<GameObject> singleRoundHit = new HashSet<GameObject>();

    public int attackDamage;
    public Color fontColor;

    private Collider2D[] weaponHits = new Collider2D[10];
    private bool enableDamage_ = false;

    public void Init(int attackDamage, Color fontColor)
    {
        this.attackDamage = attackDamage;
        this.fontColor = fontColor;
    }

    private void Awake()
    {
        weaponCollider = gameObject.GetComponent<BoxCollider2D>();
        weaponContactFilter.useTriggers = true;
        Physics2D.IgnoreCollision(weaponCollider, gameObject.transform.parent.GetComponent<BoxCollider2D>());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D()
    {

    } 

    public void OnAttack(string targetTag, bool sendDamage = true, bool sendDamagedEffect = true, bool showAttackEffect = false, bool showDamage = true, List<string> ignoringTags = null, bool visualConditional = true)
    {
        if (enableDamage)
        {
            weaponCollider.OverlapCollider(weaponContactFilter, weaponHits);
            for (int i = 0; i < weaponHits.Length; i++)
            {
                if (weaponHits[i] == null)
                {
                    continue;
                }
                bool visible = visualConditional ? VisionUtils.CanSeeEachOther(transform.parent.gameObject, weaponHits[i].gameObject, ignoringTags) : true;
                if (visible && weaponHits[i].tag == targetTag && !singleRoundHit.Contains(weaponHits[i].gameObject))
                {
                    if (sendDamage)
                    {
                        weaponHits[i].gameObject.SendMessage("ReceiveDamage", new MessageReceiveDamage(attackDamage));
                    }
                    if (sendDamagedEffect)
                    {
                        // gameObject.transform.parent.SendMessage("AttackEffect", new MessageAttackEffect(origin: transform.parent.position, target: weaponHits[i].transform.position, contactPosition: Vector3.zero));
                        weaponHits[i].gameObject.SendMessage("DamagedEffect", new MessageAttackEffect(origin: transform.parent.position, target: weaponHits[i].transform.position, contactPosition: Vector3.zero));
                    }
                    singleRoundHit.Add(weaponHits[i].gameObject);
                    if (showAttackEffect)
                    {
                        // showAttackEffect传递给攻击发起方
                        transform.parent.gameObject.SendMessage("AttackEffect", new MessageAttackEffect(origin: transform.parent.position, target: weaponHits[i].transform.position, contactPosition: Vector3.zero));
                    }
                }

            // clear after logics
            weaponHits[i] = null;
            }
        }
    }

}
