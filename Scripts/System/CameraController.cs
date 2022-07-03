using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    GameObject cowHead;

    // Start is called before the first frame update
    void Start()
    {
        cowHead = GameObject.Find("CowHead");
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 distance = cowHead.transform.position - transform.position;
        gameObject.transform.position = new Vector3(cowHead.transform.position.x, cowHead.transform.position.y, -10);
    }
}
