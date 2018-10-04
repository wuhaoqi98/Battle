using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject[] m_TankPrefab;
    public GameObject[] P1Tanks;
    public GameObject[] P2Tanks;
    public TankManager[] m_Tanks;
    public RawImage p1;
    public RawImage p2;

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    bool isP1Chosen;
    bool isP2Chosen;
    bool isNewRound = true;

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        StartCoroutine(GameLoop());
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab[i], m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        if (isNewRound)
        {
            p1.CrossFadeAlpha(255, 0, true);
            p2.CrossFadeAlpha(255, 0, true);
        }
        while (!isP1Chosen || !isP2Chosen)
        {
            yield return null;
            ChooseTank();
            m_MessageText.text = "CHOOSE YOUR TANK";
        }
        if (isNewRound)
        {
            SpawnAllTanks();
            SetCameraTargets();
            isNewRound = false;
        }
        p1.CrossFadeAlpha(0, 0, true);
        p2.CrossFadeAlpha(0, 0, true);

        ResetAllTanks();
        DisableTankControl();
        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = string.Empty;
        while (!OneTankLeft())
        {
            yield return null; 
        }    
    }


    private IEnumerator RoundEnding()
    {
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;
        m_GameWinner = GetGameWinner();
        if(m_GameWinner != null)
        {
            isP1Chosen = false;
            isP2Chosen = false;
            isNewRound = true;
        }
        string message = EndMessage();
        m_MessageText.text = message;
        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    void ChooseTank()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_TankPrefab[0] = P1Tanks[0];
            isP1Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            m_TankPrefab[0] = P1Tanks[1];
            isP1Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            m_TankPrefab[0] = P1Tanks[2];
            isP1Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            m_TankPrefab[0] = P1Tanks[3];
            isP1Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            m_TankPrefab[0] = P1Tanks[4];
            isP1Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            m_TankPrefab[0] = P1Tanks[5];
            isP1Chosen = true;
        }

        if (Input.GetKey(KeyCode.Alpha7))
        {
            m_TankPrefab[1] = P2Tanks[0];
            isP2Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha8))
        {
            m_TankPrefab[1] = P2Tanks[1];
            isP2Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha9))
        {
            m_TankPrefab[1] = P2Tanks[2];
            isP2Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Alpha0))
        {
            m_TankPrefab[1] = P2Tanks[3];
            isP2Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Minus))
        {
            m_TankPrefab[1] = P2Tanks[4];
            isP2Chosen = true;
        }
        else if (Input.GetKey(KeyCode.Equals))
        {
            m_TankPrefab[1] = P2Tanks[5];
            isP2Chosen = true;
        }
    }
}