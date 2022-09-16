using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisplayController : MonoBehaviour
{

    public GameObject bloodSpreadObj;
    public GameObject meatImpactObj;
    public GameObject blastEffectObj;

    public GameObject rainDropObj;

    public void DrawBloodSpread(Vector3 position, Vector3 rotation)
    {
        GameObject blood = Instantiate(bloodSpreadObj, gameObject.transform);
        blood.GetComponent<SpriteRenderer>().sortingLayerName = "GroundStuff";
        blood.transform.position = position;
        blood.transform.up = rotation;
    }

    public void PlayMeatImpactEffect(Vector3 position)
    {
        GameObject impact = Instantiate(meatImpactObj, position, Quaternion.identity);
        Destroy(impact, 5);
    }

    public void PlayBlastEffect(Vector3 position)
    {
        GameObject blast = Instantiate(blastEffectObj, position, Quaternion.identity);
        Destroy(blast, 5);
    }

    public void PlayRainDrop(Vector3 position)
    {
        GameObject raindrop = Instantiate(rainDropObj, position, Quaternion.identity);
        Destroy(raindrop, 2);
    }
}
