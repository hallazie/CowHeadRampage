using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    public SceneController sceneController;

    public bool activated;

    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(2f);
        sceneController.LoadNextScene();
    }

}
