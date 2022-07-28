using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
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

    public void OnAttack(string targetTag, bool sendDamage = true, bool sendAttackEffect = true, bool showDamage = true)
    {
        if (enableDamage)
        {
            weaponCollider.OverlapCollider(weaponContactFilter, weaponHits);
            for (int i = 0; i < weaponHits.Length; i++)
            {
                if (weaponHits[i] != null && weaponHits[i].gameObject != null && weaponHits[i].gameObject.tag == transform.parent.tag)
                {
                    continue;
                }
                if (weaponHits[i] != null)
                    print("current parent: " + transform.parent.name + ", tag: " + transform.parent.tag + ", sending damage to: " + weaponHits[i].name + ", tag: " + weaponHits[i].tag + " != " + targetTag);
                if (weaponHits[i] != null && weaponHits[i].tag == targetTag && !singleRoundHit.Contains(weaponHits[i].gameObject))
                {
                    if (showDamage)
                    {
                        FloatingTextManager.instance.ShowBasic("-" + attackDamage.ToString(), fontColor, gameObject.transform.position, Vector3.up * 64, duration: 2f, fontSize: 32);
                    }
                    if (sendDamage)
                    {
                        
                        weaponHits[i].gameObject.SendMessage("ReceiveDamage", new MessageReceiveDamage(attackDamage));
                    }
                    if (sendAttackEffect)
                    {
                        gameObject.transform.parent.SendMessage("AttackEffect", new MessageAttackEffect(origin: transform.parent.position, target: weaponHits[i].transform.position, contactPosition: Vector3.zero));
                    }
                    singleRoundHit.Add(weaponHits[i].gameObject);
                }

            // clear after logics
            weaponHits[i] = null;
            }
        }
    }

}
