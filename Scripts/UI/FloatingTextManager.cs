using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    // Singleton Pattern
    private static FloatingTextManager _instance;
    public static FloatingTextManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject textContainer;
    public GameObject textPrefab;

    private List<FloatingText> floatingTextPool = new List<FloatingText>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (FloatingText floatingText in floatingTextPool)
        {
            if (floatingText.active)
            {
                floatingText.UpdateFloatingText();
            }
        }
    }

    private FloatingText GetFloatingText()
    {
        foreach (FloatingText floatingText in floatingTextPool)
        {
            if (!floatingText.active)
            {
                return floatingText;
            }
        }
        FloatingText newFloatingText = new FloatingText();
        newFloatingText.gameObj = Instantiate(textPrefab);
        newFloatingText.gameObj.transform.SetParent(textContainer.transform);
        newFloatingText.text = newFloatingText.gameObj.GetComponent<Text>();
        floatingTextPool.Add(newFloatingText);
        return newFloatingText;
    }

    public void ShowBasic(string message, Color color, Vector3 position, Vector3 motion, float duration = 2f, int fontSize = 16)
    {
        FloatingText floatingText = GetFloatingText();
        floatingText.text.text = message;
        floatingText.text.fontSize = fontSize;
        floatingText.text.color = color;
        floatingText.gameObj.transform.position = Camera.main.WorldToScreenPoint(position);
        floatingText.motion = motion;
        floatingText.duration = duration;
        floatingText.Show();
    }

    public void ShowConstant(string message, Color color, Vector3 position, int fontSize = 16)
    {
        FloatingText floatingText = GetFloatingText();
        floatingText.text.text = message;
        floatingText.text.fontSize = fontSize;
        floatingText.text.color = color;
        floatingText.constant = true;
        floatingText.gameObj.transform.position = Camera.main.WorldToScreenPoint(position);
        floatingText.Show();
    }
}
