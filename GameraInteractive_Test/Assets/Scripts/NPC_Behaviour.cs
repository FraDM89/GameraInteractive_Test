using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum npcType { villager, merchant, standardEnemy}

 [RequireComponent(typeof(NavMeshAgent))]
public class NPC_Behaviour : MonoBehaviour
{
    public npcType npcType;

    public Transform[] waypoints;
    public Transform raycastPoint;
    public float timeOnWaypoint = 1f;
    public float enemyFieldOfView;
    public float rotSpeed = 0.5f;


    private NavMeshAgent npcAgent;
    private SphereCollider npcCollider;
    private bool patroling;
    private bool readyToInteract, playerInteract;
    private bool searchPlayer;
    private int currentWaypoint = 0;
    private float startTimeOnWaypoint;
    private float startAgentSpeed;
    

    void Awake()
    {
        npcAgent = GetComponent<NavMeshAgent>();
        npcCollider = GetComponent<SphereCollider>();
    }

    void Start()
    {
        startTimeOnWaypoint = timeOnWaypoint;
        startAgentSpeed = npcAgent.speed;
        patroling = true;
    }
    
    void Update()
    {
        if (patroling)
        {
            if (waypoints.Length > 0)
                PatrolingSystem();
            else if (waypoints.Length == 0)
                patroling = false;
        }

        if (readyToInteract && Input.GetKeyDown(KeyCode.Space))
        {
            TalkingSystem();
            patroling = false;
            npcAgent.speed = 0;
            //npcAgent.isStopped = true;
            playerInteract = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (npcType)
            {
                case npcType.villager:
                    readyToInteract = true;
                    break;
                case npcType.merchant:
                    readyToInteract = true;
                    break;
                case npcType.standardEnemy:
                    searchPlayer = true;
                    break;
                default:
                    print("Error?");
                    break;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            patroling = !patroling;
            readyToInteract = false;
            searchPlayer = false;
            npcAgent.speed = startAgentSpeed;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 playerDirection = other.transform.position - transform.position;
            float anglePlayerDirection = Vector3.Angle(playerDirection, transform.forward);

            if (anglePlayerDirection < enemyFieldOfView / 2 && raycastPoint != null && searchPlayer)
            {
                RaycastHit hit;
                Ray ray = new Ray(raycastPoint.position, playerDirection.normalized);

                if(Physics.Raycast(ray, out hit, npcCollider.radius))
                {
                    if (hit.collider.CompareTag("Player"))
                    {                        
                        Debug.DrawLine(raycastPoint.position, hit.transform.position, Color.green);
                    }
                }
            }

            if (readyToInteract && playerInteract)
            {
                print("guardami");
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDirection), Time.deltaTime * rotSpeed);
            }
        }

        
    }

    void PatrolingSystem()
    {
        npcAgent.destination = waypoints[currentWaypoint].position;

        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) <= 0.5f)
        {
            if (timeOnWaypoint <= 0)
            {
                if (currentWaypoint < waypoints.Length)
                    currentWaypoint++;
                if (currentWaypoint == waypoints.Length)
                    currentWaypoint = 0;
                timeOnWaypoint = startTimeOnWaypoint;
            }
            else
            {
                timeOnWaypoint -= Time.deltaTime;
            }
        }        
    }

    void TalkingSystem()
    {
        if(npcType == npcType.villager)
            print("Hello stranger! I used to be an adventurer like you, then I took an arrow to the knee..");
        if (npcType == npcType.merchant)
            print("Hello adventurer! Want to buy something?");
    }
}
