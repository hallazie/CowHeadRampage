using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistController : MonoBehaviour
{

    public BoxCollider2D fistCollider;
    public ContactFilter2D fistContactFilter;

    public bool enableGrab = false;

    private Collider2D[] fistHits;

    private void Awake()
    {
        fistCollider = GetComponent<BoxCollider2D>();
        fistContactFilter.useTriggers = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GrabableItem OnGrab()
    {
        if (enableGrab)
        {
            fistHits = new Collider2D[10];
            fistCollider.OverlapCollider(fistContactFilter, fistHits);
            for (int i = 0; i < fistHits.Length; i++)
            {
                if (fistHits[i] == null)
                {
                    continue;
                }
                if (fistHits[i].gameObject.GetComponent<GrabableItem>() != null)
                {
                    GrabableItem item = fistHits[i].gameObject.GetComponent<GrabableItem>();
                    if (item.activate && item.enabled)
                    {
                        return item;
                    } 
                }
            // clear after logics
            fistHits[i] = null;
            }
        }
        return null;
    }
}
