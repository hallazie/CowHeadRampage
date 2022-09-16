using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownShadow : MonoBehaviour
{
    public Material shadowMaterial;
    public bool staticObj = true;

    public Vector2 baseDir = new Vector2(1, -1);
    public float shadowSize = 0.2f;

    void Start()
    {
        shadowMaterial = GetComponent<SpriteRenderer>().material;
        OffsetShadow();
    }

    void Update()
    {
        if (staticObj)
        {
            return;
        }
        OffsetShadow();
    }

    private void OffsetShadow()
    {
        float degree = transform.rotation.eulerAngles.z;
        float sine = Mathf.Sin(degree * Mathf.PI / 180);
        Vector2 direct = (Quaternion.Euler(0, 0, degree) * Vector2.down);
        Vector2 offset = new Vector2(direct.x - 2 * sine, direct.y);
        Vector2 normed = offset * shadowSize;
        shadowMaterial.SetVector("_ShadowOffset", (Vector4)normed);
    }
}
