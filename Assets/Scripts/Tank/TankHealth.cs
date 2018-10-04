using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public float healthRecover;
    [HideInInspector]public bool m_Dead;
    [HideInInspector]public int ORpos;
    [HideInInspector]
    public float m_CurrentHealth;

    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
      

    float timer;

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
        m_Slider.maxValue = m_StartingHealth;
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1f && healthRecover != 0 && m_CurrentHealth <= m_StartingHealth - healthRecover)
        {
            m_CurrentHealth += healthRecover;
            SetHealthUI();
            timer = 0f;
        }
    }

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= amount;
        SetHealthUI();
        if(m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    public void OnDeath()
    {
        if (gameObject.tag == "P1-7")
        {
            GameManager1.p1OilRigs--;
            GameManager1.p1ORPositions[ORpos] = 0;
        }
        else if (gameObject.tag == "P2-7")
        {
            GameManager1.p2OilRigs--;
            GameManager1.p2ORPositions[ORpos] = 0;
        }

        if (gameObject.tag == "P1-6")
            GameManager1.p2Wins = true;
        else if (gameObject.tag == "P2-6")
            GameManager1.p1Wins = true;

        m_Dead = true;
        if (GetComponent<TankExp>())
        {
            GetComponent<TankExp>().Explode();
        }
        else
        {
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive(true);
            m_ExplosionParticles.Play();
            m_ExplosionAudio.Play();
        }
        gameObject.SetActive(false);
    }
}