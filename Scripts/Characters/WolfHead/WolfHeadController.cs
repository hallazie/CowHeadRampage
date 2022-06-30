using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfHeadController : MonoBehaviour
{

    public GameObject cowHead;
    public Animator animator;

    public float runSpeed = 1f;
    public float visionRange = 3f;

    // Start is called before the first frame update
    void Start()
    {
        cowHead = GameObject.Find("CowHead");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = cowHead.transform.position - gameObject.transform.position;
        if (distance.magnitude < visionRange)
        {
            distance = distance.normalized;
            Vector3 target = new Vector3(distance.x * runSpeed * Time.deltaTime, distance.y * runSpeed * Time.deltaTime, 0);
            transform.up = target.normalized;
            transform.position += target;            
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }


}
