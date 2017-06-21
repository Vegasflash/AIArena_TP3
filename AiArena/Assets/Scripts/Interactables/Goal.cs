using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    public void Init(int aId)
    {
        m_SpriteRenderer.color = GameManager.Instance.Data.PlayerColorList[aId];
    }
}
