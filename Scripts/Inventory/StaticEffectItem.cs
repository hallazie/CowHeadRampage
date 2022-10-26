using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEffectItem : MonoBehaviour
{

    public Sprite[] spriteList = new Sprite[0];
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; 
    }

    public void Init(MessagePlayEffect message)
    {
        if(spriteList.Length < 1)
        {
            return;
        }
        // TODO: replace with normal distribution
        int index = Random.Range(0, spriteList.Length);
        float x = RandomUtil.Gaussian(0, message.radius / 3f) + message.position.x;
        float y = RandomUtil.Gaussian(0, message.radius / 3f) + message.position.y;
        spriteRenderer.sprite = spriteList[index];
        transform.position = new Vector3(x, y, 0);
        transform.up = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), 0);
        spriteRenderer.sortingLayerName = "GroundStuff";
        spriteRenderer.sortingOrder = 5;
        spriteRenderer.enabled = true;
    }

}
