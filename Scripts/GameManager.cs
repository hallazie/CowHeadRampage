using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public FatherController father;
    public EffectDisplayController effectDisplayController;
    public CameraController cameraController;
    // public SystemManager systemManager;
    // public NavGridManager navGridManager;
    public SoundController SoundController;


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
        // systemManager = new SystemManager();
    }

    public bool playerAlive = true;

    public Dictionary<string, int> layerDict = new Dictionary<string, int>{
        {"Player", 8 },
        {"Enemy", 9 },
    };


    void Start()
    {

    }

    void Update()
    {
        if (father == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R) && !father.states.alive)
        {
            playerAlive = true;
            // father.Respawn();
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
        StartCoroutine(cameraController.Shake(duration, magnitude, frameGap));
    }

    public void BroadcastEnemyHostility(int hostilityLevel = 3, float range = 10f, bool requiredVisible = false)
    {
        EnemyController[] enemyList = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemyList)
        {
            if (!enemy.states.alive)
            {
                continue;
            }
            if ((enemy.transform.position - father.transform.position).magnitude <= range)
            {
                if (enemy.states.playerVisible)
                {
                    enemy.states.hostilityLevel = hostilityLevel;
                }
                else
                {
                    enemy.states.hostilityLevel = hostilityLevel - 1;
                }
                continue;
            }
        }
    }

}
