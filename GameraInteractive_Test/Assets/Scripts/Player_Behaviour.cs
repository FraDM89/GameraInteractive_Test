using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player_Behaviour : MonoBehaviour
{
    private NavMeshAgent m_myNavMeshAgent;
    private Animator m_myAnim;
    private Vector3 m_playerMovement;
    private float m_turnMovement;

    public LayerMask terrainLayer;
    
    void Awake()
    {
        m_myNavMeshAgent = GetComponent<NavMeshAgent>();
        m_myAnim = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        m_playerMovement = m_myNavMeshAgent.desiredVelocity.normalized;
        m_playerMovement = transform.InverseTransformDirection(m_playerMovement);
        m_turnMovement = Mathf.Atan2(m_playerMovement.x, m_playerMovement.z);
        m_myAnim.SetFloat("ver", m_playerMovement.z, 0.1f, Time.deltaTime);
        m_myAnim.SetFloat("hor", m_turnMovement, 0.1f, Time.deltaTime);
        float m_turnSpeedExtra = Mathf.Lerp(180, 360, m_playerMovement.z);
        transform.Rotate(0, m_turnSpeedExtra * m_turnMovement * Time.deltaTime, 0);

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseHitPoint = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(mouseHitPoint, out hit, 1000, terrainLayer))
            {
                m_myNavMeshAgent.destination = hit.point;
            }
        }
    }
}
