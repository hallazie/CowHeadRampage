using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonBasic : MonoBehaviour
{

    public Vector2 originalPosition;
    public float magnitude = 3f;
    private bool clicked;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (clicked)
        {
            Shake();
        }
    }

    public void Shake()
    {
        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;
        transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, transform.position.z);
    }

    public void OnStartClicked()
    {
        StartCoroutine(OnStartClickedCoroutine());
    }

    private IEnumerator OnStartClickedCoroutine()
    {
        clicked = true;
        yield return new WaitForSeconds(1f);
        SceneController.instance.LoadSpecifiedScene(0);
    }

}
