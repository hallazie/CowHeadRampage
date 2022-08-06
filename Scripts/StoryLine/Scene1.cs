using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1 : BaseScene
{

    public PigHeadController pighead;

    public const int sceneIndex = 1;
    

    void Update()
    {
        if (!pighead.states.alive)
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
