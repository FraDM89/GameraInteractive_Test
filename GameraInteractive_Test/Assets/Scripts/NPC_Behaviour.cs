using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum npcType {villager, merchant, standardEnemy}

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
    private NPC_CheckPlayer checkPlayer;
    private SphereCollider npcCollider;
    private Animator npcAnim;
    private bool patroling;
    private bool readyToInteract, playerInteract;
    private bool searchPlayer;    
    private int currentWaypoint = 0;
    private Vector3 npcMovement;
    private float turnMovement;
    private float startTimeOnWaypoint;
    private float startAgentSpeed;
    

    void Awake()
    {
        npcAgent = GetComponent<NavMeshAgent>();
        npcCollider = GetComponent<SphereCollider>();
        npcAnim = GetComponentInChildren<Animator>();
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
            playerInteract = true;
        }

        if((npcType == npcType.standardEnemy || npcType == npcType.villager) && npcAnim != null)
        {
            npcMovement = npcAgent.desiredVelocity.normalized;
            npcMovement = transform.InverseTransformDirection(npcMovement);            
            turnMovement = Mathf.Atan2(npcMovement.x, npcMovement.z);
            npcAnim.SetFloat("ver", npcMovement.z, 0.5f, Time.deltaTime);
            npcAnim.SetFloat("hor", turnMovement, 0.5f, Time.deltaTime);
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
                    npcAnim.SetTrigger("wave");
                    break;
                case npcType.standardEnemy:
                    searchPlayer = true;
                    break;
                default:
                    print("nothing");
                    break;
            }
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
                        Debug.DrawRay(raycastPoint.position, playerDirection.normalized * npcCollider.radius, Color.green);                        
                    }
                }
            }

            // villagers look at the player when he start interacting with them (double check, one for the npc, one for the player).
            if (readyToInteract && playerInteract)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(playerDirection), Time.deltaTime * rotSpeed);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // when player is outside enemies or villager range, npc start again patroling
        if (other.CompareTag("Player"))
        {
            patroling = true;
            readyToInteract = false;
            playerInteract = false;
            searchPlayer = false;            
            npcAgent.speed = startAgentSpeed;
        }
    }

    // patroling function for both enemies and villagers npc, use timeOnWaypoint variable (time) to stop npc 
    void PatrolingSystem()
    {
        // set waypoint destination from array
        npcAgent.destination = waypoints[currentWaypoint].position;

        // check if npc is near to the next destination, after he move to the next one
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
