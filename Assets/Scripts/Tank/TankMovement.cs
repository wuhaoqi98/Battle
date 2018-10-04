using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;             
    [HideInInspector] public float tankVelocity;
    public bool isAI;
    public bool baseAI;
    [HideInInspector]public float distance;
    public float targetRange;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;
    [HideInInspector]public GameObject player;
    UnityEngine.AI.NavMeshAgent nav;
    GameObject[] gameObjects;
    bool targetIsFound;
    Vector3 AIposition = new Vector3(67, 0, 0);
    Vector3 AIposition2 = new Vector3(80, 0, -10);

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        if (m_PlayerNumber == 3)
        {
            m_MovementAxisName = "Vertical1";
        }
        else if (m_PlayerNumber == 4)
            m_MovementAxisName = "Vertical2";
        if (m_PlayerNumber == 3)
            m_TurnAxisName = "Horizontal1";
        else if (m_PlayerNumber == 4)
            m_TurnAxisName = "Horizontal2";
    }


    private void Update()
    {
        if (m_PlayerNumber == 3 || (m_PlayerNumber == 4 && !baseAI))
        {
            PlayerControl();
        }

        if (isAI && !GameManager1.p1Wins && !GameManager1.p2Wins)
        {
            AIControl();
        }

        if (baseAI && !GameManager1.p1Wins && !GameManager1.p2Wins)
        {
            BaseAIControl();
        }       
    }

    void BaseAIControl()
    {
        FindTarget(2);
        distance = Vector3.Distance(transform.position, player.transform.position);
        var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * (180 / nav.angularSpeed));
        nav.SetDestination(AIposition);
        if (GetComponent<TankHealth>().m_CurrentHealth < 1000)
        {
            nav.SetDestination(AIposition2);
        }
    }

    void AIControl()
    {
        if (!targetIsFound)
        {
            FindTarget(m_PlayerNumber);
        }
        distance = Vector3.Distance(transform.position, player.transform.position);
        nav.SetDestination(player.transform.position);
        var rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * (180 / nav.angularSpeed));
        if (player.GetComponent<TankHealth>().m_Dead)
        {
            targetIsFound = false;
        }
    }

    void FindTarget(int playerNumber)
    {
        int x = 0;

        if (playerNumber == 1)
        {
            x = 2;
            player = GameObject.FindGameObjectWithTag("P2-6");
        }
        else if (playerNumber == 2)
        {
            x = 1;
            player = GameObject.FindGameObjectWithTag("P1-6");
        }

        for (int n = 1; n <= 7; n++)
        {
            if (GameObject.FindGameObjectWithTag("P" + x + "-" + n))
            {
                gameObjects = GameObject.FindGameObjectsWithTag("P" + x + "-" + n);
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    if (Vector3.Distance(transform.position, gameObjects[i].transform.position) <= targetRange)
                    {
                        if (n <= 4)
                        {
                            targetIsFound = true;
                        }
                        player = gameObjects[i];
                        n = 8;                       
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();       
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    void PlayerControl()
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        if (m_MovementInputValue > 0)
            tankVelocity = m_Speed;
        else if (m_MovementInputValue < 0)
            tankVelocity = -m_Speed;
        else
            tankVelocity = 0;
    }
}