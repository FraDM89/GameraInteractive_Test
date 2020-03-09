using System.Collections;
using UnityEngine;

public class NPC_CheckPlayer : MonoBehaviour
{
    public float minRange = 1f;
    public float midRange = 5f;
    public float maxRange = 10f;
    public float outOfSightRange = 30f;
    public bool drawRangeDebug;
    [HideInInspector]
    public bool playerNearby, playerClose, playerAttached;

    private GameObject m_player;
    private GameObject m_thisNPC;
    private Animator m_myAnim;
    private NPC_Behaviour m_myNpcBehaviour;
    private bool m_checkDistance = true;
    private float m_playerDistance;
    WaitForSeconds delay = new WaitForSeconds(1);

    
    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_thisNPC = gameObject.transform.GetChild(0).gameObject;
        m_myAnim = GetComponent<Animator>();
        m_myNpcBehaviour = GetComponent<NPC_Behaviour>();
    }

    void Start()
    {
        StartCoroutine(CheckDistance());
    }

    void Update()
    {
        // check range distance from NPC to player
        if (m_playerDistance > maxRange && m_playerDistance < outOfSightRange)
        {
            // NPC inside camera view but not close enough to activate actions (attacks, interactions,...)

            playerNearby = false;
            playerClose = false;
            playerAttached = false;

            m_thisNPC.SetActive(true);
            if (m_myAnim != null)
                m_myAnim.enabled = true;
            if (m_myNpcBehaviour != null)
                m_myNpcBehaviour.enabled = true;
        }
        else if(m_playerDistance < maxRange && m_playerDistance > midRange)
        {
            // player in range to NPC (use this for long range attacks or far interactions)

            playerNearby = true;
            playerClose = false;
            playerAttached = false;
        }
        else if (m_playerDistance < midRange && m_playerDistance > minRange)
        {
            // player close to NPC (use this for mid range attacks or close interactions)

            playerNearby = false;
            playerClose = true;
            playerAttached = false;
        }         
        else if ( m_playerDistance < minRange)
        {
            // player attached to NPC (use this for melee attacks or very close interactions)

            playerNearby = false;
            playerClose = false;
            playerAttached = true;
        } 
        else if (m_playerDistance > outOfSightRange)
        {
            // player far far away from NPC (not visible in camera)

            m_thisNPC.SetActive(false);
            if (m_myAnim != null)
                m_myAnim.enabled = false;
            if(m_myNpcBehaviour != null)
                m_myNpcBehaviour.enabled = false;
        }
    }

    // coroutine to check distance from player (not every frame)
    // block coroutine with m_checkDistance bool variable
    IEnumerator CheckDistance()
    {
        while (m_checkDistance)
        {
            m_playerDistance = Vector3.Distance(transform.position, m_player.transform.position);             

            yield return delay;         
        }
    }

    private void OnDrawGizmos()
    {
        if (drawRangeDebug)
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
