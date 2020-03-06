using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player_Behaviour : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;

    public LayerMask terrainLayer;
    
    void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseHitPoint = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(mouseHitPoint, out hit, 1000, terrainLayer))
            {
                myNavMeshAgent.destination = hit.point;
            }
        }
    }
}
