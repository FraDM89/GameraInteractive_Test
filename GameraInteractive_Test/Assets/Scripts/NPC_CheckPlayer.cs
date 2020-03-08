using System.Collections;
using UnityEngine;

public class NPC_CheckPlayer : MonoBehaviour
{
    public float minRange, midRange, maxRange, outOfSightRange;
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
        if (m_playerDistance > maxRange && m_playerDistance < outOfSightRange)
        {
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
            playerNearby = true;
            playerClose = false;
            playerAttached = false;
        }
        else if (m_playerDistance < midRange && m_playerDistance > minRange)
        {
            playerNearby = false;
            playerClose = true;
            playerAttached = false;
        }         
        else if ( m_playerDistance < minRange)
        {
            playerNearby = false;
            playerClose = false;
            playerAttached = true;
        } 
        else if (m_playerDistance > outOfSightRange)
        {
            m_thisNPC.SetActive(false);
            if (m_myAnim != null)
                m_myAnim.enabled = false;
            if(m_myNpcBehaviour != null)
                m_myNpcBehaviour.enabled = false;
        }
    }

    // coroutine to check distance from player (not every frame)
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
