using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour {

    public bool AI;
    int AIrandom;
    float AITimer;
    int AIResource = 700;
    int AIResource2 = 800;
    int AIResource3 = 1400;
    int AIStage;

    public GameObject[] p1Tanks;
    public GameObject[] p2Tanks;
    public GameObject oilTank;
    public Transform oilTankSpawn;
    public Transform[] p1Spawns;
    public Transform[] p2Spawns;
    public Transform[] p1ORSpawns;
    public Transform[] p2ORSpawns;
    public Text p1Res;
    public Text p2Res;
    public Text winText;
    public Text[] p1CDText;
    public Text[] p2CDText;
    public int startResource = 500;
    [HideInInspector]public static int p1OilRigs;
    [HideInInspector]public static int p2OilRigs;
    [HideInInspector]public static bool p1Wins;
    [HideInInspector]public static bool p2Wins;

    float[] p1Timer = new float[7];
	float[] p2Timer = new float[7];
    [HideInInspector]public static int p1Resource;
    [HideInInspector]public static int p2Resource;
    int p1Product = 20;
    int p2Product = 20;
    int random;
    float timer;
    float oilTankTimer;
    float[] p1TankCDs = new float[7];
    int[] p1TankCosts = new int[7];
    float[] p2TankCDs = new float[7];
    int[] p2TankCosts = new int[7];
    public static int[] p1ORPositions = new int[3];
    public static int[] p2ORPositions = new int[3];

    float[] armyCDs = {1, 3, 10, 15, 20, 30, 50 };
    int[] armyCosts = {100, 200, 400, 500, 750, 1500, 1000 };
    float[] merCDs = {1, 3, 10, 15, 25, 30, 50 };
    int[] merCosts = {100, 200, 300, 500, 900, 1200, 1000 };

    void Start () {
        p1Resource = startResource;
        p2Resource = startResource;
        p1Wins = false;
        p2Wins = false;

        p1TankCDs = merCDs;
        p1TankCosts = merCosts;
        p2TankCDs = armyCDs;
        p2TankCosts = armyCosts;
    }
	
	// Update is called once per frame
	void Update () {
        if (p1Wins)
            winText.text = "P1 WINS";
        else if (p2Wins)
            winText.text = "P2 WINS";

        ResourceControl();
        TimerControl();
        OilTankControl();

        if (Input.GetKeyUp(KeyCode.I))
        {
            GameObject base2 = GameObject.FindGameObjectWithTag("P2-6");
            AI = !AI;
            if (AI)
            {
                base2.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                base2.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
            }
            else
            {
                base2.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                base2.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
            }
            base2.GetComponent<TankMovement>().baseAI = !base2.GetComponent<TankMovement>().baseAI;
        }

        random = (int)Random.Range(0f, 2.99f);

        P1Control();
        if (!AI)
        {
            P2Control();
        }
        else if (AI && !p1Wins && !p2Wins)
        {
            AIControl();
        }
    }
    
    void TimerControl()
    {
        for (int n = 0; n < p1Timer.Length; n++)
        {
            p1Timer[n] += Time.deltaTime;
            p2Timer[n] += Time.deltaTime;
            if (p1TankCDs[n] - p1Timer[n] >= 0)
                p1CDText[n].text = "" + ((int)(p1TankCDs[n] - p1Timer[n]) + 1);
            else
                p1CDText[n].text = "";
            if (p2TankCDs[n] - p2Timer[n] >= 0)
                p2CDText[n].text = "" + ((int)(p2TankCDs[n] - p2Timer[n]) + 1);
            else
                p2CDText[n].text = "";
        }
    }

    void ResourceControl()
    {
        timer += Time.deltaTime;
        if (timer >= 0.8f)
        {
            p1Resource += p1Product + 10 * p1OilRigs;
            p2Resource += p2Product + 10 * p2OilRigs;
            p1Res.text = "$" + p1Resource;
            p2Res.text = "$" + p2Resource;
            timer = 0f;
        }
    }

    void OilTankControl()
    {
        oilTankTimer += Time.deltaTime;
        if (oilTankTimer >= OilStorage.SpawnTime && !GameObject.FindGameObjectWithTag("OilTank"))
        {
            Instantiate(oilTank, oilTankSpawn.position, oilTankSpawn.rotation);
            oilTankTimer = 0f;
        }
    }

    void P1Control()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            P1Spawn(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            P1Spawn(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            P1Spawn(2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            P1Spawn(3);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            P1Spawn(4);
        }
        if (Input.GetKey(KeyCode.Alpha6))
        {
            P1Spawn(5);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            P1Spawn(6);
        }
    }

    void P2Control()
    {
        if (Input.GetKey(KeyCode.Alpha7))
        {
            P2Spawn(0);
        }
        if (Input.GetKey(KeyCode.Alpha8))
        {
            P2Spawn(1);
        }
        if (Input.GetKey(KeyCode.Alpha9))
        {
            P2Spawn(2);
        }
        if (Input.GetKey(KeyCode.Alpha0))
        {
            P2Spawn(3);
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            P2Spawn(4);
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            P2Spawn(5);
        }
        if (Input.GetKey(KeyCode.RightShift))
        {
            P2Spawn(6);
        }
    }

    void P1Spawn(int n)
    {
        if(n == 6)
        {
            if (p1Timer[6] >= p1TankCDs[6] && p1Resource >= p1TankCosts[6] && p1ORPositions[random] == 0)
            {
                p1Tanks[6].GetComponent<TankHealth>().ORpos = random;
                Instantiate(p1Tanks[6], p1ORSpawns[random].position, p1ORSpawns[random].rotation);
                p1Timer[6] = 0f;
                p1Resource -= p1TankCosts[6];
                p1OilRigs++;
                p1ORPositions[random] = 1;
            }      
        }
        else if (p1Timer[n] >= p1TankCDs[n] && p1Resource >= p1TankCosts[n])
        {
            Instantiate(p1Tanks[n], p1Spawns[random].position, p1Spawns[random].rotation);
            p1Timer[n] = 0f;
            p1Resource -= p1TankCosts[n];
        }
    }

    void P2Spawn(int n)
    {
        if(n == 6)
        {
            if (p2Timer[6] >= p2TankCDs[6] && p2Resource >= p2TankCosts[6] && p2ORPositions[random] == 0)
            {
                p2Tanks[6].GetComponent<TankHealth>().ORpos = random;
                Instantiate(p2Tanks[6], p2ORSpawns[random].position, p2ORSpawns[random].rotation);
                p2Timer[6] = 0f;
                p2Resource -= p2TankCosts[6];
                p2OilRigs++;
                p2ORPositions[random] = 1;
            }
        }
        else if (p2Timer[n] >= p2TankCDs[n] && p2Resource >= p2TankCosts[n])
        {
            Instantiate(p2Tanks[n], p2Spawns[random].position, p2Spawns[random].rotation);
            p2Timer[n] = 0f;
            p2Resource -= p2TankCosts[n];
        }
    }

    void AIControl()
    {
        AITimer += Time.deltaTime;
        if(p2OilRigs == 0)
        {
            if(AIResource >= 0 && AITimer >= 2f)
            {
                AIrandom = (int)Random.Range(0f, 2.9f);
                if (p2Timer[AIrandom] >= p2TankCDs[AIrandom] && p2Resource >= p2TankCosts[AIrandom])
                    AIResource -= p2TankCosts[AIrandom];
                P2Spawn(AIrandom);               
                AITimer = 0f;
            }
            if(AIResource < 0)
            {
                P2Spawn(6);
            }
        }
        else if (p2OilRigs == 1)
        {
            if(AIResource2 >= 0 && AITimer >= 2f)
            {
                AIrandom = (int)Random.Range(0f, 3.9f);
                if (p2Timer[AIrandom] >= p2TankCDs[AIrandom] && p2Resource >= p2TankCosts[AIrandom])
                    AIResource2 -= p2TankCosts[AIrandom];
                P2Spawn(AIrandom);
                
                AITimer = 0f;
            }
            if(AIResource2 < 0)
            {
                P2Spawn(6);
            }
        }
        else if (p2OilRigs == 2)
        {
            if (AIResource3 >= 0 && AITimer >= 2f)
            {
                AIrandom = (int)Random.Range(0f, 4.9f);
                if (p2Timer[AIrandom] >= p2TankCDs[AIrandom] && p2Resource >= p2TankCosts[AIrandom])
                    AIResource3 -= p2TankCosts[AIrandom];
                P2Spawn(AIrandom);
                
                AITimer = 0f;
            }
            if (AIResource2 < 0)
            {
                P2Spawn(6);
            }
        }
        else if(p2OilRigs == 3 && AIStage != 4)
        {
            if(p2Resource >= 1500)
            {
                P2Spawn(5);
                AIStage = 4;
            }
        }
        else if(p2OilRigs == 3 && AIStage == 4)
        {
            if (p2Resource > 1000 && AITimer >= 1f)
            {
                AIrandom = (int)Random.Range(0f, 6.9f);
                P2Spawn(AIrandom);
                AITimer = 0;
            }
            else if (AITimer >= 2f)
            {
                AIrandom = (int)Random.Range(0f, 5.9f);
                P2Spawn(AIrandom);
                AITimer = 0;
            }
            if(p2Resource >= 1500)
            {
                P2Spawn(5);
            }
        }   
    }
}
