using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSelectorManager : MonoBehaviour
{

    public GameObject target;
    public CowHeadController cowHead;

    public bool active = false;
    public Vector3 offset = new Vector3(0.5f, 0.5f, 0);
    public float currentTargetDistance = 1000f;

    private static InteractionSelectorManager _instance;
    public static InteractionSelectorManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
    }

    public void Update()
    {
        if (!active)
            return;
        if (target != null) {
            currentTargetDistance = ((Vector2)(cowHead.transform.position - target.transform.position)).magnitude;
        }
        if (target.layer == GameManager.instance.layerDict["Enemy"] && target != null && Input.GetKeyDown(KeyCode.E))
        {
            target.SendMessage("StartConversation");
            target.transform.up = (Vector2)(cowHead.transform.position - target.transform.position);
        }
    }

    public void StartIntereaction(GameObject target)
    {
        float newDistance = ((Vector2)(cowHead.transform.position - target.transform.position)).magnitude;

        if (target != this.target && (this.target == null || newDistance < currentTargetDistance))
        {
            this.target = target;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            active = true;
            transform.position = target.transform.position + offset;
            currentTargetDistance = ((Vector2)(cowHead.transform.position - this.target.transform.position)).magnitude;
        }
        else if(target == this.target)
        {
            transform.position = target.transform.position + offset;
        }
    }

    public void StopInteraction(GameObject target)
    {
        if (!active)
            return;
        if (active && target != this.target)
            return;
        this.target = null;
        active = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

}
