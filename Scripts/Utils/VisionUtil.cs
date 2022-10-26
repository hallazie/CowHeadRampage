using System.Collections.Generic;
using UnityEngine;

public static class VisionUtil
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

    public static float UnblockedDistance(GameObject obj, Vector2 direction, float targetDistance, List<string> ignoreTags = null)
    {
        float minDist = targetDistance;
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(
            origin: obj.transform.position,
            direction: direction.normalized,
            distance: targetDistance
        );
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            BoxCollider2D collider = hit.transform.GetComponent<BoxCollider2D>();
            Debug.Log("ignore: " + ignoreTags + ", current: " + hit.transform.gameObject.tag + ", " + hit.transform.gameObject.name);
            if (ignoreTags != null && ignoreTags.Contains(hit.transform.gameObject.tag))
            {
                continue;
            }
            if (collider != null)
            {
                float size = collider.size.x < collider.size.y ? collider.size.x : collider.size.y;
                float currentDist = ((Vector2)(obj.transform.position - hit.transform.position)).magnitude - size;
                minDist = currentDist < minDist ? currentDist : minDist;
            }
        }
        return minDist;
    }
}
