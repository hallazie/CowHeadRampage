using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float flyingSpeed = 20f;
    public float damageAmount = 10f;

    [HideInInspector]
    public string sourceTag;
    public string targetTag;
    public Vector3 sourcePosition;
    public Vector3 targetPosition;
    public float jitter;

    private Vector3 direction = Vector3.zero;
    private bool alive = true;

    public void Init(MessageInstantiateBullet message)
    {
        targetPosition = message.targetPosition;
        sourcePosition = message.sourcePosition;
        targetTag = message.targetTag;
        sourceTag = message.sourceTag;
        damageAmount = message.damageAmount;
        jitter = Random.Range(-message.jitter, message.jitter);


        targetPosition.z = 0;
        sourcePosition.z = 0;
        direction = (targetPosition - sourcePosition).normalized;
        Vector3 axis = Vector3.Cross(direction, Vector3.up);
        direction = (Quaternion.AngleAxis(90 * jitter, axis) * direction).normalized;
        transform.position = sourcePosition;
        transform.up = direction;
    }

    void Update()
    {
        if (!alive)
            return;
        Vector3 delta = direction * flyingSpeed * Time.deltaTime;
        transform.position += delta;
    }

    private void HideBullet()
    {
        GetComponent<SpriteRenderer>().sprite = null;
        GetComponent<BoxCollider2D>().enabled = false;
        alive = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!alive)
            return;
        if (collider.gameObject.layer == GameManager.instance.layerDict["Enemy"])
        {
            return;
        }
        if (collider.gameObject.tag == targetTag)
        {
            collider.gameObject.SendMessage("ReceiveDamage", new MessageReceiveDamage((int)damageAmount));
        }
        if (collider.transform.parent != null && collider.transform.parent.tag == targetTag)
        {
            collider.transform.parent.gameObject.SendMessage("ReceiveDamage", new MessageReceiveDamage((int)damageAmount));
        }
        GameManager.instance.effectDisplayController.PlayBlastEffect(transform.position);
        if (collider.name == "DamageCollider" && collider.gameObject.transform.parent.name == "Player")
        {
            HideBullet();
            Destroy(gameObject, 3f);
            return;
        }
        Destroy(gameObject);
    }

    public void ReceiveDamage(MessageReceiveDamage message)
    {
        GameManager.instance.ShakeCamera();
        for (int i = 0; i < 3; i++)
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
