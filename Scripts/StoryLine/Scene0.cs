using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Scene0 : BaseScene
{

    public const int sceneIndex = 0;

    void Update()
    {
        Vector2 cowHeadPos = (Vector2)GameManager.instance.cowHead.transform.position;
        Vector2 targetPos = new Vector2(-1f, 16f);
        if ((cowHeadPos - targetPos).magnitude <= 1)
        {
            SaveGameAndLoadNextScene();
        }
    }

    private void SaveGameAndLoadNextScene()
    {
        int saveSceneIndex = sceneIndex > SystemManager.instance.maxSceneReached ? sceneIndex : SystemManager.instance.maxSceneReached;
        SystemManager.instance.SaveSystemStatus(new SaveLoadData(saveSceneIndex));
        StartCoroutine(LoadScene());
    }

}
