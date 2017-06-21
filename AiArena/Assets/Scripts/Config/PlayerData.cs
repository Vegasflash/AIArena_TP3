using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string m_PlayerName;

    [Header("Skills Points")]
    [SerializeField] private int m_FasterMove = 0;
    [SerializeField] private int m_FasterTurn = 0;
    [SerializeField] private int m_ImprovedShield = 0;
    [SerializeField, Tooltip("Additional stun duration in seconds")] private int m_ExtraStun = 0;
    [SerializeField, Tooltip("Additional weapon length")] private int m_ExtraWeaponLength = 0;
    [SerializeField, Tooltip("Additional hearts")] private int m_ExtraLife = 0;

    public string PlayerName
    {
        get { return m_PlayerName; }
    }
    
    public int FasterMove
    {
        get { return m_FasterMove; }
    }

    public int FasterTurn
    {
        get { return m_FasterTurn; }
    }

    public int ImprovedShield
    {
        get { return m_ImprovedShield; }
    }

    public int ExtraStun
    {
        get { return m_ExtraStun; }
    }

    public int ExtraWeaponLength
    {
        get { return m_ExtraWeaponLength; }
    }

    public int ExtraLife
    {
        get { return m_ExtraLife; }
    }

    #region Editor

    #pragma warning disable 414
    [HideInInspector] [SerializeField] private int m_AvailablePoints = GameData.MAX_AVAILABLE_POINTS;

    #endregion
}
