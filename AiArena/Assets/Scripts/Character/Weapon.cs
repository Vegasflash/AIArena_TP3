using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite[] m_LightsaberVisualList;

    private BasePlayer m_BasePlayer;

    public void Init(int aIndex, BasePlayer aBasePlayer)
    {
        m_BasePlayer = aBasePlayer;
        m_SpriteRenderer.sprite = m_LightsaberVisualList[aIndex];

        Vector3 scale = transform.localScale;
        scale.x = Player.WeaponLength;
        transform.localScale = scale;
    }

    public void SetActive(bool aEnable)
    {
        gameObject.SetActive(aEnable);
    }

    public BasePlayer Player
    {
        get
        {
            return m_BasePlayer;
        }
    }
}
