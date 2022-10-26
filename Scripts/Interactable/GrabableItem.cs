using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableItem : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public float mass = 10f;
    public bool activate = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(spriteRenderer.sprite.bounds.extents.x, spriteRenderer.sprite.bounds.extents.y);
    }

    void Start()
    {
    
    }

    void Update()
    {
        
    }

    public void Grabbed()
    {
        // spriteRenderer.enabled = false;
        // Destroy(transform.gameObject, 5f);
    }

}
