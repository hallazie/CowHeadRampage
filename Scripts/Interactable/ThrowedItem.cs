using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowedItem : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;

    private bool hit = false;
    private Vector2 direction;
    private float distance = 100f;
    private float translateSpeed = 80f;
    private float rotateSpeed = 360f;
    private Vector3 initPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        initPosition = transform.position;
    }

    void Start()
    {
        
    }

    void Update()
    {
        //if (hit)
        //{
        //    return;
        //}
        //float throwedDistance = (initPosition - transform.position).magnitude;
        //if (throwedDistance >= distance)
        //{
        //    return;
        //}
        //transform.position += (Vector3)(direction * Time.deltaTime * translateSpeed);
        //transform.rotation *= Quaternion.Euler(new Vector3(0f, 0f, Time.deltaTime * rotateSpeed));
    }

    private void FixedUpdate()
    {
        if (hit)
        {
            return;
        }
        float throwedDistance = (initPosition - transform.position).magnitude;
        if (throwedDistance >= distance)
        {
            return;
        }
        transform.position += (Vector3)(direction * Time.deltaTime * translateSpeed);
        transform.rotation *= Quaternion.Euler(new Vector3(0f, 0f, Time.deltaTime * rotateSpeed));
    }

    public void Init(MessageThrowItem item)
    {
        float xSize = item.sprite.bounds.extents.x;
        float ySize = item.sprite.bounds.extents.y;
        boxCollider.size = new Vector2(xSize, ySize);
        spriteRenderer.sprite = item.sprite;
        direction = item.throwDirection;
        float collideDistance = VisionUtil.UnblockedDistance(gameObject, direction, distance, ignoreTags: new List<string> { "Player", "Weapon", "ThrowableItem" });
        Debug.Log("dist1: " + collideDistance + ", dist2: " + distance);
        distance = distance < collideDistance ? distance : collideDistance;
    }
}
