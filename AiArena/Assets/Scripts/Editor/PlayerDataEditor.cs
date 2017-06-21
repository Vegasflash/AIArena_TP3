using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerData))]
public class PlayerDataEditor : Editor
{
    private SerializedProperty m_PlayerNameSP;
    private SerializedProperty m_FasterMoveSP;
    private SerializedProperty m_FasterTurnSP;
    private SerializedProperty m_ImprovedShieldSP;
    private SerializedProperty m_ExtraStunSP;
    private SerializedProperty m_ExtraWeaponLengthSP;
    private SerializedProperty m_ExtraLifeSP;

    private SerializedProperty m_AvailableSkillPointsSP;

    void OnEnable()
    {
        m_PlayerNameSP = serializedObject.FindProperty("m_PlayerName");
        m_FasterMoveSP = serializedObject.FindProperty("m_FasterMove");
        m_FasterTurnSP = serializedObject.FindProperty("m_FasterTurn");
        m_ImprovedShieldSP = serializedObject.FindProperty("m_ImprovedShield");
        m_ExtraStunSP = serializedObject.FindProperty("m_ExtraStun");
        m_ExtraWeaponLengthSP = serializedObject.FindProperty("m_ExtraWeaponLength");
        m_ExtraLifeSP = serializedObject.FindProperty("m_ExtraLife");

        m_AvailableSkillPointsSP = serializedObject.FindProperty("m_AvailablePoints");
    }

    public override void OnInspectorGUI()
    {
        PlayerData playerData = (PlayerData) target;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Player Name", GUILayout.MaxWidth(160));
        m_PlayerNameSP.stringValue = EditorGUILayout.TextField(m_PlayerNameSP.stringValue);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // -----------------
        EditorGUILayout.LabelField("Skill points", EditorStyles.boldLabel);
        
        DisplaySkill("Faster Move", m_FasterMoveSP);
        DisplaySkill("Faster Turn", m_FasterTurnSP);
        DisplaySkill("Improved Shield", m_ImprovedShieldSP);
        DisplaySkill("Extra Stun", m_ExtraStunSP);
        DisplaySkill("Extra Weapon Length", m_ExtraWeaponLengthSP);
        DisplaySkill("Extra Life", m_ExtraLifeSP);

        GUILayout.Space(15);


        EditorGUILayout.LabelField("Available points: "+m_AvailableSkillPointsSP.intValue);
        if (GUILayout.Button("Reset", GUILayout.MaxWidth(100)))
        {
            m_AvailableSkillPointsSP.intValue = GameData.MAX_AVAILABLE_POINTS;
            m_FasterMoveSP.intValue = 0;
            m_FasterTurnSP.intValue = 0;
            m_ImprovedShieldSP.intValue = 0;
            m_ExtraStunSP.intValue = 0;
            m_ExtraWeaponLengthSP.intValue = 0;
            m_ExtraLifeSP.intValue = 0;
        }

        serializedObject.ApplyModifiedProperties();
    }


    private void DisplaySkill(string aName, SerializedProperty aProp)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(aName, GUILayout.MaxWidth(160));
        for (int i = 0; i < GameData.MAX_SKILL_LEVEL; ++i)
        {
            if (i < aProp.intValue)
            {
                EditorGUILayout.LabelField("●", GUILayout.MaxWidth(12));
            }
            else
            {
                EditorGUILayout.LabelField("◊", GUILayout.MaxWidth(12));
            }
        }

        int newValue = aProp.intValue;
        if (GUILayout.Button("-", GUILayout.MaxWidth(20)) && newValue > 0 && m_AvailableSkillPointsSP.intValue < GameData.MAX_AVAILABLE_POINTS)
        {
            newValue--;
            m_AvailableSkillPointsSP.intValue++;
        }

        if (GUILayout.Button("+", GUILayout.MaxWidth(20)) && newValue < GameData.MAX_SKILL_LEVEL && m_AvailableSkillPointsSP.intValue > 0)
        {
            newValue++;
            m_AvailableSkillPointsSP.intValue--;
        }

        newValue = Mathf.Clamp(newValue, 0, GameData.MAX_SKILL_LEVEL);

        aProp.intValue = newValue;
        EditorGUILayout.EndHorizontal();
    }
}
