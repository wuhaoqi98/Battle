using UnityEngine;
using System.Collections;

public class Reaper : MonoBehaviour {

    public float damage;
    public float attackDistance;
    public float timeBetweenFires;
    public float maxSpeed;
    public float minSpeed;
    public float targetRange;

    float timer;
    UnityEngine.AI.NavMeshAgent nav;
    TankMovement tankMovement;

	// Use this for initialization
	void Start () {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        tankMovement = GetComponent<TankMovement>();
        targetRange = tankMovement.targetRange;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= timeBetweenFires)
        {
            if(tankMovement.distance <= attackDistance)
            {
                tankMovement.player.GetComponent<TankHealth>().TakeDamage(damage);
            }
            timer = 0f;
        }

        if(tankMovement.distance <= targetRange && tankMovement.distance >= nav.stoppingDistance + 2)
        {
            nav.speed = maxSpeed;
        }
        else
        {
            nav.speed = minSpeed;
        }

    }
}
