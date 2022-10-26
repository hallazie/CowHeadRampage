using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // public FatherController father;
    public PlayerController player;
    public float maxOverlookDistance = 15f;
    public float cameraMoveSpeed = 20f;

    private float pivotShift = 0.5f;
    private bool startOverlook = false;
    private bool keepOverlook = false;

    public float cameraHeight = -30f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        startOverlook = Input.GetKeyDown(KeyCode.LeftShift);
        keepOverlook = Input.GetKey(KeyCode.LeftShift) && !startOverlook;
        UpdateView();
    }

    private void SmoothMoveCamera()
    {
        // use lerp
    }

    public IEnumerator Shake(float duration, float magnitude, int frameGap)
    {
        float elapsed = 0f;
        int counter = 0;
        while (elapsed < duration)
        {
            counter += 1;
            elapsed += Time.deltaTime;
            if (counter % frameGap != 0)
            {
                Vector3 position = GameManager.instance.player.transform.position;
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                transform.position = new Vector3(position.x + x, position.y + y, transform.position.z);
            }
            yield return null;
        }
    }

    private void UpdateView()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = (mousePosition - (Vector2)transform.position);
        if (startOverlook)
        {
            Vector3 nextPosition = new Vector3(mousePosition.x, mousePosition.y, cameraHeight);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, nextPosition, Time.deltaTime * cameraMoveSpeed);
        }
        else if (keepOverlook)
        {
            Vector2 middlePosition = (mousePosition + (Vector2)player.transform.position) / 2f;
            Vector3 nextPosition = new Vector3(middlePosition.x, middlePosition.y, cameraHeight);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, nextPosition, Time.deltaTime * cameraMoveSpeed);
        }
        else
        {
            Vector2 lookAt = distance.normalized;
            Vector3 nextPosition = new Vector3(player.transform.position.x + pivotShift * lookAt.x, player.transform.position.y + pivotShift * lookAt.y, cameraHeight);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, nextPosition, Time.deltaTime * cameraMoveSpeed);
        }
    }
}
