using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private static HUD m_Instance;

    public static HUD Instance
    {
        get { return m_Instance; }
    }

    [SerializeField] private RectTransform m_PlayerContainer;
    [SerializeField] private PanelPlayer m_PanelPlayerPrefab;

    [SerializeField] private Text m_TxtTimeLeft;

    private List<PanelPlayer> m_PanelPlayerList;

    private void Awake()
    {
        m_Instance = this;

        m_PanelPlayerList = new List<PanelPlayer>();
    }

    public void AddPlayer(BasePlayer aPlayer)
    {
        PanelPlayer panel = Instantiate(m_PanelPlayerPrefab);
        panel.transform.SetParent(m_PlayerContainer, false);

        panel.Init(aPlayer);

        m_PanelPlayerList.Add(panel);
    }

    public void UpdateTimeLeft(float aTimeleft)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(aTimeleft);
        m_TxtTimeLeft.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    public void UpdatePlayerLife(int aId, int aLife)
    {
        m_PanelPlayerList[aId].UpdateLife(aLife);
    }

    public void IncrementObjective(int aId)
    {

    }
}
