using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPlayer : MonoBehaviour
{
    [SerializeField] private RectTransform m_PanelLifeContainer;
    [SerializeField] private PanelLife m_PanelLifePrefab;

    [SerializeField] private Text m_TxtPlayerName;
    [SerializeField] private Image m_ImgTank;

    private List<PanelLife> m_PanelLifeList;

    public void Init(BasePlayer aBasePlayer)
    {
        m_TxtPlayerName.text = aBasePlayer.Data.PlayerName;
        m_ImgTank.color = GameManager.Instance.Data.PlayerColorList[aBasePlayer.Id];

        // TODO User Player HP
        m_PanelLifeList = new List<PanelLife>();
        for (int i = 0; i < aBasePlayer.Life; i++)
        {
            PanelLife panel = Instantiate(m_PanelLifePrefab);
            panel.transform.SetParent(m_PanelLifeContainer, false);

            panel.SetState(PanelLife.EState.Filled);
            m_PanelLifeList.Add(panel);
        }
    }

    public void UpdateLife(int aLife)
    {
        for (int i = 0; i < m_PanelLifeList.Count; i++)
        {
            m_PanelLifeList[i].SetState(i < aLife ? PanelLife.EState.Filled : PanelLife.EState.Empty);
        }

    }
}
