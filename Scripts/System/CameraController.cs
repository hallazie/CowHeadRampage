using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public CowHeadController cowHead;
    public float maxOverlookDistance = 15f;
    public float cameraMoveSpeed = 20f;

    private float pivotShift = 0.25f;
    private bool startOverlook = false;
    private bool keepOverlook = false;

    public float cameraHeight = -20f;

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

    private void UpdateView()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = (mousePosition - (Vector2)transform.position);
        if (startOverlook)
        {
            Vector3 nextPosition = new Vector3(mousePosition.x, mousePosition.y, cameraHeight);
            // gameObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, cameraHeight);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, nextPosition, Time.deltaTime * cameraMoveSpeed);
        }
        else if (keepOverlook)
        {
            Vector2 middlePosition = (mousePosition + (Vector2)cowHead.transform.position) / 2f;
            Vector3 nextPosition = new Vector3(middlePosition.x, middlePosition.y, cameraHeight);
            // gameObject.transform.position = new Vector3(middlePosition.x, middlePosition.y, cameraHeight);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, nextPosition, Time.deltaTime * cameraMoveSpeed);
        }
        else
        {
            Vector2 lookAt = distance.normalized;
            Vector3 nextPosition = new Vector3(cowHead.transform.position.x + pivotShift * lookAt.x, cowHead.transform.position.y + pivotShift * lookAt.y, cameraHeight);
            // gameObject.transform.position = new Vector3(cowHead.transform.position.x + pivotShift * lookAt.x, cowHead.transform.position.y + pivotShift * lookAt.y, cameraHeight);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, nextPosition, Time.deltaTime * cameraMoveSpeed);
        }
    }
}
