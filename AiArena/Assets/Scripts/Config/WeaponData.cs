using UnityEngine;

[CreateAssetMenu(fileName = "new WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    [SerializeField, Tooltip("The length of the laser")] private float m_LaserLength = 1f;
    [SerializeField] private float m_StunDuration = 1f;
    
    public float LaserLength
    {
        get
        {
            return m_LaserLength;
        }
    }

    public float StunDuration
    {
        get
        {
            return m_StunDuration;
        }
    }
}
