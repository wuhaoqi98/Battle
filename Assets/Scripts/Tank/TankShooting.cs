using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform[] m_FireTransform;          
    public AudioSource m_ShootingAudio;      
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f;
    public float timeBetweenFires = 0.5f;
    public bool singleFire;
    
    [HideInInspector] public float m_CurrentLaunchForce;
    public float randomFireAngle = 0f;
    public bool changeForce;

    private string m_FireButton;          
    float timer = 0.1f;
    UnityEngine.AI.NavMeshAgent nav;
    TankMovement tankMovement;
    int n;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        tankMovement = GetComponent<TankMovement>();
        m_PlayerNumber = tankMovement.m_PlayerNumber;
    }

    private void Start()
    {
        if(m_PlayerNumber == 3)
            m_FireButton = "Fire1";
        else if (m_PlayerNumber == 4)
            m_FireButton = "Fire2";
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if ((tankMovement.isAI || tankMovement.baseAI) && !GameManager1.p1Wins && !GameManager1.p2Wins)
        {
            AIControl();
        }
        if (!tankMovement.isAI)
        {
            PlayerControl();
        }

        /*for(int i = 0; i < m_FireTransform.Length; i++)
        {
            if (timer < 0.1f && m_FireTransform[i].GetComponent<Light>())
            {
                m_FireTransform[i].GetComponent<Light>().enabled = true;
            }
            else if(m_FireTransform[i].GetComponent<Light>())
            {
                m_FireTransform[i].GetComponent<Light>().enabled = false;
            }
        }*/
    }

    void PlayerControl()
    {
        if (timer >= timeBetweenFires)
        {
            if (Input.GetButtonDown(m_FireButton))
            {
                m_CurrentLaunchForce = m_MinLaunchForce + tankMovement.tankVelocity;
                Fire();
                timer = 0f;
            }
        }
    }

    void AIControl()
    {
        if (timer >= timeBetweenFires && (tankMovement.distance <= nav.stoppingDistance))
        {
            if (changeForce)
            {
                m_CurrentLaunchForce = m_MinLaunchForce * (tankMovement.distance / nav.stoppingDistance) + nav.speed * 0.8f;
            }
            else
            {
                m_CurrentLaunchForce = m_MinLaunchForce + nav.speed * 0.8f;
            }
            timer = 0f;

            if (singleFire)
            {
                SingleFire();
            }
            else
                Fire();
            
        }       
    }

    private void Fire()
    {
        for (int i = 0; i < m_FireTransform.Length; i++)
        {
            if(randomFireAngle != 0)
            {
                m_FireTransform[i].transform.localRotation = Quaternion.Euler(0f,
                    Random.Range(-randomFireAngle, randomFireAngle), 0f);
            }

            Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform[i].position, m_FireTransform[i].rotation) as Rigidbody;
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform[i].forward;
        }
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    void SingleFire()
    {
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform[n].position, m_FireTransform[n].rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform[n].forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        if (n >= (m_FireTransform.Length - 1))
            n = 0;
        else
            n++;
    }
}