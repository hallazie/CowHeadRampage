using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSlaveController : MonoBehaviour
{

    private Animator animator;
    public string animationName;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (animationName != "")
        {
            animator.Play(animationName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
