using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnvironment : MonoBehaviour
{

    public Sprite[] spriteList = new Sprite[0];
    public int spriteIndex = -1;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteIndex < spriteList.Length && spriteIndex >= 0)
        {
            spriteRenderer.sprite = spriteList[spriteIndex];
        }
        else
        {
            int index = Random.Range(0, spriteList.Length);
            spriteRenderer.sprite = spriteList[index];
        }
    }

}
