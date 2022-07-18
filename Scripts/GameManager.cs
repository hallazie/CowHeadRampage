using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public CowHeadController cowHead;

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

    }

    public bool playerAlive = true;

    public Dictionary<string, int> layerDict = new Dictionary<string, int>{
        {"Player", 8 },
        {"Enemy", 9 },
    };


    void Start()
    {
        // Cursor.visible = false;
    }

    void Update()
    {
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

    public void BroadcastEnemyHostility()
    {
        WolfHeadController[] wolfHeadList = FindObjectsOfType<WolfHeadController>();
        foreach (WolfHeadController wolfHead in wolfHeadList)
        {
            if (wolfHead.states.playerVisible)
            {
                wolfHead.states.hostilityLevel = 3;
            }
        }
    }

}
