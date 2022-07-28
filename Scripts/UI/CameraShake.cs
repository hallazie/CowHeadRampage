using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public IEnumerator Shake (float duration, float magnitude, int frameGap)
    {
        float elapsed = 0f;
        int counter = 0;
        while (elapsed < duration)
        {
            counter += 1;
            elapsed += Time.deltaTime;
            if (counter % frameGap != 0)
            {
                Vector3 position = GameManager.instance.cowHead.transform.position;
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                transform.position = new Vector3(position.x + x, position.y + y, transform.position.z);
            }
            yield return null;
        }
    }
}
