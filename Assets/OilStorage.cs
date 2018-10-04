using UnityEngine;
using System.Collections;

public class OilStorage : MonoBehaviour {

    public int resource;
    public static float SpawnTime = 60f;

    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TankMovement>())
        {
            TankMovement tankMovement = other.GetComponent<TankMovement>();
            if (tankMovement.m_PlayerNumber == 1 || tankMovement.m_PlayerNumber == 3)
                GameManager1.p1Resource += resource;
            if (tankMovement.m_PlayerNumber == 2 || tankMovement.m_PlayerNumber == 4)
                GameManager1.p2Resource += resource;
            Destroy(gameObject);
        }
    }
}
