using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public GameObject throwingObjectPrefab;

    public float mass;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ActivateByGrabableItem(GrabableItem item)
    {
        try
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = item.spriteRenderer.sprite;
            mass = item.mass; 
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void ThrowAtDirection(Vector2 direction, float power)
    {
        GameObject throwingObject = Instantiate(throwingObjectPrefab, gameObject.transform.position, gameObject.transform.rotation);
        throwingObject.name = "ThrowingObject";
        // throwingObject.tag = "Bullet";

        MessageThrowItem message = new MessageThrowItem(spriteRenderer.sprite, 10, direction, mass, power);
        throwingObject.SendMessage("Init", message);

        spriteRenderer.enabled = false;
    }

}
