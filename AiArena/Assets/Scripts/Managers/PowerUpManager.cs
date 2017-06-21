using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager m_Instance;
    public static PowerUpManager Instance
    {
        get
        {
            return m_Instance;
        }
    }
	
    [SerializeField] private PowerUp m_PowerUpPrefab;
    [SerializeField] private Transform[] m_PowerUpSpawnPointList;

    [Header("Game Design")]
    [SerializeField] private float m_PowerUpSpawnInterval;

    private List<PowerUp> m_PowerUpList;

    private float m_Counter;
    
    private void Awake()
    {
        m_Instance = this;

        m_PowerUpList = new List<PowerUp>();
    }
   
    private void Update()
    {
        m_Counter += Time.deltaTime;

        if (m_Counter >= m_PowerUpSpawnInterval)
        {
            m_Counter -= m_PowerUpSpawnInterval;
            SpawnPowerUp();
        }
    }

    public void Shuffle<T>(T[] aArray)
    {
        for (int i = 0; i < aArray.Length; i++)
        {
            T tmp = aArray[i];
            int r = Random.Range(i, aArray.Length);
            aArray[i] = aArray[r];
            aArray[r] = tmp;
        }
    }

    private void SpawnPowerUp()
    {
        Shuffle<Transform>(m_PowerUpSpawnPointList);
        for (int i = 0; i < m_PowerUpSpawnPointList.Length; ++i)
        {
            Transform spawnPoint = m_PowerUpSpawnPointList[i];
            if (spawnPoint.childCount == 0)
            {
                PowerUp powerUp = Instantiate(m_PowerUpPrefab, spawnPoint, false);
                m_PowerUpList.Add(powerUp);
                return;
            }
        }
    }
    public void RemovePowerUp(PowerUp aPowerUp)
    {
        m_PowerUpList.Remove(aPowerUp);
        Destroy(aPowerUp.gameObject);
    } 

    public List<Vector3> GetPowerUpPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (PowerUp powerUp in m_PowerUpList)
        {
            positions.Add(powerUp.transform.position);
        }

        return positions;
    }
}
