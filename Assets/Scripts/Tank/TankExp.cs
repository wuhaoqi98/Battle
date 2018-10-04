using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankExp : MonoBehaviour {
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 100f;
    public float m_ExplosionRadius = 5f;
    public float ExplosionDistance = 3f;
    public int playerNum;

    TankMovement tankMovement;


    private void Start()
    {
        tankMovement = GetComponent<TankMovement>();
    }

    void Update()
    {
        if (tankMovement.distance <= ExplosionDistance)
        {
            GetComponent<TankHealth>().TakeDamage(200f);
        }
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
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
