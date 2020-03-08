﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum npcType {villager, merchant, standardEnemy}

 [RequireComponent(typeof(NavMeshAgent))]
public class NPC_Behaviour : MonoBehaviour
{
    public npcType npcType;
    public Transform raycastPoint;
    public Transform[] waypoints;
    public float timeOnWaypoint = 1f;
    public float npcFieldOfView;
    public float rotSpeed = 100f;
    public float attackRatio;

    private NavMeshAgent m_npcAgent;
    private NPC_CheckPlayer m_npcCheckPlayer;
    private SphereCollider m_npcCollider;
    private Animator m_npcAnim;
    private bool m_patroling;
    private bool m_readyToInteract, m_playerInteract;
    private bool m_searchPlayer;
    private bool m_isAttacking;
    private int m_currentWaypoint = 0;
    private Vector3 m_npcMovement;
    private float m_turnMovement;
    private float m_startTimeOnWaypoint;
    private float m_startAgentSpeed;
    WaitForSeconds m_attackRatio;

    void Awake()
    {
        m_npcAgent = GetComponent<NavMeshAgent>();
        m_npcCheckPlayer = GetComponent<NPC_CheckPlayer>();
        m_npcCollider = GetComponent<SphereCollider>();
        m_npcAnim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        m_startTimeOnWaypoint = timeOnWaypoint;
        m_startAgentSpeed = m_npcAgent.speed;
        m_patroling = true;
        m_attackRatio = new WaitForSeconds(attackRatio);
    }
    
    void Update()
    {
        // movement depends if there are more than one waypoint
        if (m_patroling)
        {
            if (waypoints.Length > 0)
                PatrolingSystem();
            else if (waypoints.Length == 0)
                m_patroling = false;
        }

        //
        if (m_readyToInteract && Input.GetKeyDown(KeyCode.Space))
        {
            TalkingSystem();
            m_patroling = false;
            m_npcAgent.speed = 0;
            m_playerInteract = true;
        }

        // (basic animator controller) calculate speed from moving agent to animate blendTree, please check animator variables name
        if((npcType == npcType.standardEnemy || npcType == npcType.villager) && m_npcAnim != null)
        {
            m_npcMovement = m_npcAgent.desiredVelocity.normalized;
            m_npcMovement = transform.InverseTransformDirection(m_npcMovement);
            m_turnMovement = Mathf.Atan2(m_npcMovement.x, m_npcMovement.z);

            m_npcAnim.SetFloat("ver", m_npcMovement.z, 0.5f, Time.deltaTime);
            m_npcAnim.SetFloat("hor", m_turnMovement, 0.5f, Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // when player enter inside NPC zone he trigger NPC main behaviour (ready to search or ready to interact)
        if (other.CompareTag("Player"))
        {
            switch (npcType)
            {
                case npcType.villager:                    
                    m_searchPlayer = true;
                    break;
                case npcType.merchant:
                    m_readyToInteract = true;
                    m_npcAnim.SetTrigger("wave");
                    break;
                case npcType.standardEnemy:
                    m_searchPlayer = true;
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
            // first check player vector direction and angle
            Vector3 playerDirection = other.transform.position - transform.position;
            float anglePlayerDirection = Vector3.Angle(playerDirection, transform.forward);

            // player vector direction inside enemy field of view angle
            if (anglePlayerDirection < npcFieldOfView / 2 && raycastPoint != null && m_searchPlayer)
            {
                RaycastHit hit;
                Ray ray = new Ray(raycastPoint.position, playerDirection.normalized);

                if(Physics.Raycast(ray, out hit, m_npcCollider.radius))
                {
                    // another control to check if NPC see player without occlusions (walls for example)
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.DrawRay(raycastPoint.position, playerDirection.normalized * m_npcCollider.radius, Color.green);
                        m_readyToInteract = true;

                        // based on the type of enemy and the distance from the player, NPC decide how to move and attack (melee, distance, ecc)
                        if (npcType == npcType.standardEnemy && !m_isAttacking)
                        {
                            m_patroling = false;
                            m_npcAgent.destination = hit.point;

                            if(m_npcCheckPlayer.playerAttached)
                                StartCoroutine(AttackPlayer());
                        }
                    }
                }
            }

            // villagers look at the player when he start interacting with them (double check, one for the npc, one for the player).
            if (m_readyToInteract && m_playerInteract && npcType == npcType.villager)
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
            m_patroling = true;
            m_readyToInteract = false;
            m_playerInteract = false;
            m_searchPlayer = false;
            m_npcAgent.speed = m_startAgentSpeed;
        }
    }

    // patroling function for both enemies and villagers npc, use timeOnWaypoint variable (time) to stop npc 
    void PatrolingSystem()
    {
        // set waypoint destination from array
        m_npcAgent.destination = waypoints[m_currentWaypoint].position;

        // check if npc is near to the next destination, after he move to the next one
        if (Vector3.Distance(transform.position, waypoints[m_currentWaypoint].position) <= 0.5f)
        {
            if (timeOnWaypoint <= 0)
            {
                if (m_currentWaypoint < waypoints.Length)
                    m_currentWaypoint++;
                if (m_currentWaypoint == waypoints.Length)
                    m_currentWaypoint = 0;
                timeOnWaypoint = m_startTimeOnWaypoint;
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
    
    IEnumerator AttackPlayer()
    {
        m_isAttacking = true;
        print("Never Should've Come Here!");
        yield return m_attackRatio;
        m_isAttacking = false;
    }
}
