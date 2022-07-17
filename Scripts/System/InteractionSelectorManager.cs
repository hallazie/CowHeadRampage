using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSelectorManager : MonoBehaviour
{

    public GameObject target;
    public CowHeadController cowHead;

    public bool active = false;
    public Vector3 offset = new Vector3(1, 1, 0);
    public float currentTargetDistance;

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
    }

    public void StartIntereaction(GameObject target)
    {
        if (target == this.target && active)
            return;
        if (target != this.target && ((Vector2)(cowHead.transform.position - target.transform.position)).magnitude < currentTargetDistance)
            return;
        this.target = target;
        active = true;
        this.gameObject.SetActive(active);
        this.transform.position = target.transform.position + offset;
    }

    public void StopInteraction()
    {
        if (!active)
            return;
        target = null;
        active = false;
        this.gameObject.SetActive(active);
    }

}
