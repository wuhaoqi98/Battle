using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                              
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;
    public int playerNum;

    GameObject[] gameObjects;
    GameObject[] otherShells;


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);

    }

    void Update()
    {
        for (int n = 1; n <= 7; n++)
        {
            gameObjects = GameObject.FindGameObjectsWithTag("P" + playerNum + "-" + n);
            for (int i = 0; i < gameObjects.Length; i++)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), gameObjects[i].GetComponent<Collider>());
            }
        }
            otherShells = GameObject.FindGameObjectsWithTag("Shell");
            for (int i = 0; i < otherShells.Length; i++)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), otherShells[i].GetComponent<Collider>());
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRB = colliders[i].GetComponent<Rigidbody>();
            if (!targetRB)
            {
                continue;
            }
            TankHealth targetHealth = targetRB.GetComponent<TankHealth>();
            if (!targetHealth)
                continue;
            float damage = CalculateDamage(targetRB.position);
            targetHealth.TakeDamage(damage);
        }
            m_ExplosionParticles.transform.parent = null;
            m_ExplosionParticles.Play();
            m_ExplosionAudio.Play();
            Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
            Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        float expDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - expDistance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;
        damage = Mathf.Max(0f, damage);
        return damage;
    }
}