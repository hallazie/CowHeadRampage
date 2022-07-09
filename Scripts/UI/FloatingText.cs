using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText
{

    public bool active;
    public GameObject gameObj;
    public Text text;
    public Vector3 motion;
    public float duration;
    public float lastShown;

    public void Show()
    {
        active = true;
        lastShown = Time.time;

        gameObj.SetActive(active);
    }

    public void Hide()
    {
        active = false;

        gameObj.SetActive(active);
    }

    public void PlayTextAnimation()
    {
        FloatingDisappear();
    }

    public void UpdateFloatingText()
    {
        if (!active)
            return;

        if (Time.time - lastShown >= duration)
            Hide();

        PlayTextAnimation();
    }

    private void FloatingDisappear()
    {
        gameObj.transform.position += motion * Time.deltaTime;
    }
}
