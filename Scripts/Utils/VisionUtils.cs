using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VisionUtils
{

    public static bool CanSeeEachOther(GameObject object1, GameObject object2, List<string> ignoringTags = null)
    {
        Vector2 position1 = object1.transform.position;
        Vector2 position2 = object2.transform.position;
        Vector2 direction = position2 - position1;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(
            origin: position1,
            direction: direction.normalized,
            distance: direction.magnitude
        );
        for (int i = 0; i < hits.Length; i++)
        {
            string name = hits[i].transform.gameObject.name;
            string tag = hits[i].transform.gameObject.tag;
            int layer = hits[i].transform.gameObject.layer;
            if (tag == "Transparent")
            {
                continue;
            }
            if (tag == object1.tag || tag == object2.tag || ignoringTags != null && ignoringTags.Contains(tag))
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

}
