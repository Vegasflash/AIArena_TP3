using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get { return m_Instance; }
    }
    
    [SerializeField] private GameData m_GameData;
    [SerializeField] private Ball m_BallPrefab;
    [SerializeField] private BasePlayer[] m_PlayerPrefabList;
    [SerializeField] private Transform[] m_PlayerGoalList;

    private List<BasePlayer> m_PlayerList;
    private Ball m_Ball;

    public System.Action<int> OnObjectiveCompleteCallback;

    private float m_GameTime;

    private void Awake()
    {
        m_Instance = this;

        AddBall();
        AddPlayers();

        InitGoals();

        m_GameTime = Data.GameDuration;
    }

    private void Update()
    {
        foreach (BasePlayer player in m_PlayerList)
        {
            if (player.Life > 0)
            {
                player.OnUpdate();
            }
        }

        m_GameTime -= Time.deltaTime;
        HUD.Instance.UpdateTimeLeft(m_GameTime);
    }
    
    private void AddPlayers()
    {
        m_PlayerList = new List<BasePlayer>();

        for (var i = 0; i < m_PlayerPrefabList.Length; i++)
        {
            BasePlayer player = Instantiate(m_PlayerPrefabList[i]);
            player.transform.position = m_PlayerGoalList[i].position;

            Vector3 forward = Vector3.zero - player.transform.position;
            float rotationZ = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            player.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotationZ));

            player.Init(i);

            m_PlayerList.Add(player);
            HUD.Instance.AddPlayer(player);
        }
    }

    private void AddBall()
    {
        m_Ball = Instantiate(m_BallPrefab);
        m_Ball.transform.position = Vector3.zero;
    }

    private void InitGoals()
    {
        for (var i = 0; i < m_PlayerGoalList.Length; i++)
        {
            m_PlayerGoalList[i].GetComponent<Goal>().Init(i);
        }
    }

    public void ObjectiveComplete(int aId)
    {
        OnObjectiveCompleteCallback(aId);
    }

    public int GetPlayerQty()
    {
        return m_PlayerList.Count;
    }

    public int GetPlayerObjectiveCount(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerObjectiveCount aId is out of range");
            return 0;
        }

        return m_PlayerList[aId].ObjectiveCount;
    }

    public float GetPlayerMoveSpeed(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerMoveSpeed aId is out of range");
            return 0f;
        }

        return m_PlayerList[aId].MoveSpeed;
    }

    public float GetPlayerTurnSpeed(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerTurnSpeed aId is out of range");
            return 0f;
        }

        return m_PlayerList[aId].TurnSpeed;
    }

    public float GetPlayerShieldDuration(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerShieldDuration aId is out of range");
            return 0f;
        }

        return m_PlayerList[aId].ShieldDuration;
    }

    public float GetPlayerWeaponLength(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerWeaponLength aId is out of range");
            return 0f;
        }

        return m_PlayerList[aId].WeaponLength;
    }

    public float GetPlayerStunDuration(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerWeaponLength aId is out of range");
            return 0f;
        }

        return m_PlayerList[aId].WeaponLength;
    }

    public Vector3 GetPlayerPos(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerPos aId is out of range");
            return Vector3.zero;
        }

        return m_PlayerList[aId].transform.position;
    }

    public List<Vector3> GetAllPlayerPos()
    {
        List<Vector3> allPos = new List<Vector3>();

        foreach (BasePlayer player in m_PlayerList)
        {
            allPos.Add(player.transform.position);
        }

        return allPos;
    }
    
    public Vector3 GetPlayerDirection(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerDirection aId is out of range");
            return Vector3.zero;
        }

        return m_PlayerList[aId].transform.right;
    }

    public Vector3 GetGoalPos(int aId)
    {
        if (aId >= m_PlayerGoalList.Length)
        {
            Debug.LogWarning("GameManager::GetGoalPos aId is out of range");
            return Vector3.zero;
        }

        return m_PlayerGoalList[aId].position;
    }

    public List<Vector3> GetPowerUpPositions()
    {
        return PowerUpManager.Instance.GetPowerUpPositions();
    }

    public string GetPlayerName(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerName aId is out of range");
            return string.Empty;
        }

        return m_PlayerList[aId].PlayerName;
    }

    public int GetPlayerLife(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerHealth aId is out of range");
            return 0;
        }

        return m_PlayerList[aId].Life;
    }

    public bool GetPlayerStunStatus(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerStunStatus aId is out of range");
            return false;
        }

        return m_PlayerList[aId].IsStunned;
    }

    public bool GetPlayerShieldCooldownStatus(int aId)
    {
        if (aId >= m_PlayerList.Count)
        {
            Debug.LogWarning("GameManager::GetPlayerShieldCooldownStatus aId is out of range");
            return false;
        }

        return m_PlayerList[aId].IsShieldInCooldown;
    }

    public Vector3 GetBallPos()
    {
        return m_Ball.transform.position;
    }

    public bool IsBallPickedUp
    {
        get
        {
            return m_Ball.IsPickedUp;
        }
    }

    public int PlayerWithBall
    {
        get
        {
            return m_Ball.PlayerId;
        }
    }

    public GameData Data
    {
        get
        {
            return m_GameData;
        }
    }
}
