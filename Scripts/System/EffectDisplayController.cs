using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectDisplayController : MonoBehaviour
{

    public GameObject bloodSpreadObj;
    public GameObject bloodFlow;

    public GameObject meatImpactObj;
    public GameObject blastEffectObj;

    public GameObject groundDustDirectedObj;
    public GameObject groundDustCircleObj;

    public GameObject bulletShellObj;


    public void DrawBloodSpread(Vector3 position, Quaternion rotation, int displayNum, float radius)
    {
        for (int i = 0; i < displayNum; i++)
        {
            GameObject blood = Instantiate(bloodSpreadObj, position, Quaternion.identity);
            MessagePlayEffect message = new MessagePlayEffect(position, rotation.eulerAngles, radius);
            blood.SendMessage("Init", message);
            blood.transform.parent = transform;
        }
    }

    public void PlayBloodFlow(Vector3 position, Quaternion rotation)
    {
        GameObject blood = Instantiate(bloodFlow, position, rotation);
        blood.transform.parent = transform;
    }

    public void PlayMeatImpactEffect(Vector3 position)
    {
        GameObject impact = Instantiate(meatImpactObj, position, Quaternion.identity);
        impact.transform.parent = transform;
        Destroy(impact, 5);
    }

    public void PlayBlastEffect(Vector3 position)
    {
        GameObject blast = Instantiate(blastEffectObj, position, Quaternion.identity);
        blast.transform.parent = transform;
        Destroy(blast, 5);
    }

    public void PlayGroundDustDirected(Vector3 position, Quaternion rotation)
    {
        GameObject groundEffect = Instantiate(groundDustDirectedObj, position, rotation);
        // EffectMessage message = new EffectMessage(random get effectname);
        groundEffect.transform.parent = transform;
        Destroy(groundEffect, 2);
    }

    public void PlayGroundDustCircle(Vector3 position)
    {
        GameObject groundEffect = Instantiate(groundDustCircleObj, position, Quaternion.identity);
        groundEffect.transform.parent = transform;
        Destroy(groundEffect, 2);
    }

    public void DrawBulletShell(Vector3 position)
    {
        Vector3 jitter = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        position += jitter;
        GameObject bulletShell = Instantiate(bulletShellObj, position, Quaternion.Euler(new Vector3(0, 0, Random.Range(-180f, 180f))));
        bulletShell.transform.parent = transform;
        // Destroy(bulletShell, 30);
    }
}
