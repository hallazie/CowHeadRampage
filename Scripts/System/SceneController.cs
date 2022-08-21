using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        print("SceneController Initiated:" + this);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    public void LoadNextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSpecifiedScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

}
