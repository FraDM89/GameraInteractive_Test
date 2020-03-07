using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_CheckPlayer : MonoBehaviour
{
    public float maxRange, midRange, minRange, outOfSightRange;
    public bool drawRange_debug;
    [HideInInspector]
    public bool playerNearby, playerClose, playerAttached;

    private GameObject player;
    private GameObject thisNPC;
    private Animator myAnim;
    private NPC_Behaviour myNpcBehaviour;
    private bool checkDistance = true;
    private float playerDistance;
    WaitForSeconds delay = new WaitForSeconds(1);

    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        thisNPC = gameObject.transform.GetChild(0).gameObject;
        myAnim = GetComponent<Animator>();
        myNpcBehaviour = GetComponent<NPC_Behaviour>();
    }

    void Start()
    {
        StartCoroutine(CheckDistance());
    }

    void Update()
    {
        if (playerDistance > maxRange && playerDistance < outOfSightRange)
        {
            playerNearby = false;
            playerClose = false;
            playerAttached = false;

            thisNPC.SetActive(true);
            if (myAnim != null)
                myAnim.enabled = true;
            if (myNpcBehaviour != null)
                myNpcBehaviour.enabled = true;
        }
        else if(playerDistance < maxRange && playerDistance > midRange)
        {
            playerNearby = true;
            playerClose = false;
            playerAttached = false;
        }
        else if (playerDistance < midRange && playerDistance > minRange)
        {
            playerNearby = false;
            playerClose = true;
            playerAttached = false;
        }         
        else if ( playerDistance < minRange)
        {
            playerNearby = false;
            playerClose = false;
            playerAttached = true;
        } 
        else if (playerDistance > outOfSightRange)
        {
            thisNPC.SetActive(false);
            if (myAnim != null)
                myAnim.enabled = false;
            if(myNpcBehaviour != null)
                myNpcBehaviour.enabled = false;
        }
    }

    // coroutine to check distance from player (not every frame)
    IEnumerator CheckDistance()
    {
        while (checkDistance)
        {
            playerDistance = Vector3.Distance(transform.position, player.transform.position);             

            yield return delay;         
        }
    }

    private void OnDrawGizmos()
    {
        if (drawRange_debug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, maxRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, midRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minRange);

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, outOfSightRange); 
        }        
    }
}
