using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpreadController : MonoBehaviour
{

    public List<GameObject> bloodSpriteList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawBloodSpread(Vector3 position, Vector3 rotation)
    {
        int index = Random.Range(0, bloodSpriteList.Count);
        GameObject blood = Instantiate(bloodSpriteList[index], gameObject.transform);
        blood.transform.position = position;
        blood.transform.up = rotation;
        print("blood spread!");
    }
}
