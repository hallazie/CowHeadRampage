using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRoutine : MonoBehaviour
{

    public GameObject[] patrolRoutine = new GameObject[0];

    public List<Vector2> GetPatrolList()
    {
        List<Vector2> patrolSpotList = new List<Vector2>();
        foreach (GameObject patrolSpot in patrolRoutine)
        {
            patrolSpotList.Add((Vector2)patrolSpot.transform.position);
        }
        return patrolSpotList;
    }

}
