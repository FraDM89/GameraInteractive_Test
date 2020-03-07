using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player_Behaviour : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    private Animator myAnim;
    private Vector3 playerMovement;
    private float turnMovement;

    public LayerMask terrainLayer;
    
    void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myAnim = GetComponentInChildren<Animator>();
    }

    
    void Update()
    {
        playerMovement = myNavMeshAgent.desiredVelocity.normalized;
        playerMovement = transform.InverseTransformDirection(playerMovement);
        turnMovement = Mathf.Atan2(playerMovement.x, playerMovement.z);
        myAnim.SetFloat("ver", playerMovement.z, 0.1f, Time.deltaTime);
        myAnim.SetFloat("hor", turnMovement, 0.1f, Time.deltaTime);


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
