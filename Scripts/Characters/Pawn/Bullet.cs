using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float flyingSpeed = 0.2f;
    public float damageAmount = 10f;

    [HideInInspector]
    public string sourceTag;
    public string targetTag;
    public Vector3 sourcePosition;
    public Vector3 targetPosition;

    private Vector3 direction = Vector3.zero;
    private bool enabled = true;

    public void Init(MessageInstantiateBullet message)
    {
        targetPosition = message.targetPosition;
        sourcePosition = message.sourcePosition;
        targetTag = message.targetTag;
        sourceTag = message.sourceTag;
        damageAmount = message.damageAmount;

        targetPosition.z = 0;
        sourcePosition.z = 0;
        direction = (targetPosition - sourcePosition).normalized;
        transform.position = sourcePosition;
        transform.up = direction;
    }

    void Update()
    {
        if (!enabled)
            return;
        Vector3 delta = direction * flyingSpeed;
        transform.position += delta;
    }

    private void HideBullet()
    {
        GetComponent<SpriteRenderer>().sprite = null;
        GetComponent<BoxCollider2D>().enabled = false;
        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!enabled)
            return;
        if (collider.gameObject.layer == GameManager.instance.layerDict["Enemy"])
        {
            return;
        }
        if (collider.gameObject.tag == targetTag)
        {
            FloatingTextManager.instance.ShowBasic("-" + damageAmount.ToString(), Color.yellow, gameObject.transform.position, Vector3.up * 64, duration: 2f, fontSize: 32);
            collider.gameObject.SendMessage("ReceiveDamage", new MessageReceiveDamage((int)damageAmount));
        }
        GameManager.instance.effectDisplayController.PlayBlastEffect(transform.position);
        if (collider.name == "DamageCollider" && collider.gameObject.transform.parent.name == "CowHead")
        {
            HideBullet();
            return;
        }
        Destroy(gameObject);
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        for(int i = 0; i < 3; i++)
        {
            float jitx = Random.Range(-3, 3);
            float jity = Random.Range(-3, 3);
            Vector3 blastSpot = new Vector3(transform.position.x + jitx, transform.position.y + jity, 0);
            GameManager.instance.effectDisplayController.PlayBlastEffect(blastSpot);
        }
        Destroy(gameObject);
    }

    public void AttackEffect(MessageAttackEffect message)
    {

    }

}
