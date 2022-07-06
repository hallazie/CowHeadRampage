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

    private Collider2D[] weaponHits = new Collider2D[10];
    private bool enableDamage_ = false;

    public void Init(int attackDamage)
    {
        this.attackDamage = attackDamage;
    }

    private void Awake()
    {
        weaponCollider = gameObject.GetComponent<BoxCollider2D>();        
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

    public void OnAttack(string targetTag)
    {
        if (enableDamage)
        {
            weaponCollider.OverlapCollider(weaponContactFilter, weaponHits);
            for (int i = 0; i < weaponHits.Length; i++)
            {
                if (weaponHits[i] != null && weaponHits[i].tag == targetTag && !singleRoundHit.Contains(weaponHits[i].gameObject))
                {
                    FloatingTextManager.instance.ShowBasic("-" + attackDamage.ToString(), Color.red, gameObject.transform.position, Vector3.up * 64, duration: 2f, fontSize: 16);
                    weaponHits[i].gameObject.SendMessage("ReceiveDamage", new MessageReceiveDamage(attackDamage));
                    gameObject.transform.parent.SendMessage("AttackEffect", new MessageAttackEffect(origin: transform.position, target:weaponHits[i].transform.position));
                    singleRoundHit.Add(weaponHits[i].gameObject);
                }

            // clear after logics
            weaponHits[i] = null;
            }
        }
    }

}
