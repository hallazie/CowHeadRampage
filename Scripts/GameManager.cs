using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CowHeadController cowHead;
    public EffectDisplayController effectDisplayController;
    public CameraShake cameraShaker;
    public SystemManager systemManager;
    public NavGridManager navGridManager;


    // Singleton Pattern
    private static GameManager _instance;
    public static GameManager instance
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
        systemManager = new SystemManager();
}

public bool playerAlive = true;

    public Dictionary<string, int> layerDict = new Dictionary<string, int>{
        {"Player", 8 },
        {"Enemy", 9 },
    };


    void Start()
    {
        // Cursor.visible = false;
        Physics2D.IgnoreLayerCollision(layerDict["Enemy"], layerDict["Enemy"]);
    }

    void Update()
    {
        if (cowHead == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R) && !cowHead.states.alive) {
            playerAlive = true;
            cowHead.Respawn();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemy.SendMessage("Respawn");
            } 
        }
    }

    public void ShakeCamera(float duration = 0.15f, float magnitude = 0.05f, int frameGap = 10)
    {
        StartCoroutine(cameraShaker.Shake(duration, magnitude, frameGap));
    }

    public void BroadcastEnemyHostility(float range = 10f, bool requiredVisible = false)
    {
        WolfHeadController[] wolfHeadList = FindObjectsOfType<WolfHeadController>();
        foreach (WolfHeadController wolfHead in wolfHeadList)
        {
            if (wolfHead.states.playerVisible)
            {
                wolfHead.states.hostilityLevel = 3;
            }
            if (!requiredVisible && (wolfHead.transform.position - cowHead.transform.position).magnitude <= range)
            {
                wolfHead.states.hostilityLevel = 3;
            }
        }
    }

}
