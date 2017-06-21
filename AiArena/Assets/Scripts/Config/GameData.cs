using UnityEngine;

[CreateAssetMenu(fileName = "new GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public const int MAX_SKILL_LEVEL = 5;
    public const int MAX_AVAILABLE_POINTS = 9;

    [Header("Game")]
    [SerializeField] private Color[] m_PlayerColorList;

    [SerializeField, Tooltip("Game duration in seconds")] private float m_GameDuration;
    [SerializeField, Tooltip("PowerUp duration in seconds")] private float m_PowerUpDuration;
    [SerializeField, Tooltip("PowerUp speed multiplier")] private float m_PowerUpSpeedMultiplier;

    [Header("Stats")]    
    [SerializeField, Tooltip("Shield cooldown in seconds")] private float m_ShieldCooldown;

    [SerializeField, Tooltip("Unity units per seconds")] private float m_MoveSpeed;
    [SerializeField, Tooltip("Angles in degree per seconds")] private float m_TurnSpeed;

    [SerializeField, Tooltip("Shield duration in seconds")] private float m_ShieldDuration;
    [SerializeField, Tooltip("Stun duration in seconds")] private float m_StunDuration;
    [SerializeField, Tooltip("The length of the weapon")] private float m_WeaponLength;

    [SerializeField] private int m_BaseLife;

    [Header("Bonus Stats")]
    [SerializeField] private float m_MaxSkillBonusMove;
    [SerializeField] private float m_MaxSkillBonusTurn;
    [SerializeField] private float m_MaxSkillBonusShield;
    [SerializeField] private float m_MaxSkillBonusExtraStun;
    [SerializeField] private float m_MaxSkillBonusWeaponLength;
    [SerializeField] private float m_MaxSkillBonusLife;

    public float GameDuration
    {
        get { return m_GameDuration; }
    }

    public Color[] PlayerColorList
    {
        get { return m_PlayerColorList; }
    }

    public float WeaponLength
    {
        get { return m_WeaponLength; }
    }

    public float StunDuration
    {
        get { return m_StunDuration; }
    }

    public float MaxSkillBonusMove
    {
        get { return m_MaxSkillBonusMove; }
    }

    public float MaxSkillBonusTurn
    {
        get { return m_MaxSkillBonusTurn; }
    }

    public float MaxSkillBonusShield
    {
        get { return m_MaxSkillBonusShield; }
    }

    public float MaxSkillBonusExtraStun
    {
        get { return m_MaxSkillBonusExtraStun; }
    }

    public float MaxSkillWeaponLength
    {
        get { return m_MaxSkillBonusWeaponLength; }
    }
    public float MaxSkillBonusLife
    {
        get { return m_MaxSkillBonusLife; }
    }

    public int BaseLife
    {
        get { return m_BaseLife; }
    }

    public float ShieldCooldown
    {
        get { return m_ShieldCooldown; }
    }

    public float PowerUpDuration
    {
        get { return m_PowerUpDuration; }
    }

    public float PowerUpSpeedMultiplier
    {
        get { return m_PowerUpSpeedMultiplier; }
    }

    public float MoveSpeed
    {
        get { return m_MoveSpeed; }
    }

    public float TurnSpeed
    {
        get { return m_TurnSpeed; }
    }

    public float ShieldDuration
    {
        get { return m_ShieldDuration; }
    }
}
