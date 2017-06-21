using UnityEngine;

public class PanelLife : MonoBehaviour
{
    public enum EState
    {
        Empty,
        Filled
    }

    private const string ANIM_PARAM_STATE = "State";

    [SerializeField] private Animator m_Animator;

    private EState m_State;

    private void OnEnable()
    {
        SetState(m_State);
    }

    private void SetAnimatorInteger(string aId, int aValue)
    {
        if (m_Animator.isInitialized)
        {
            m_Animator.SetInteger(aId, aValue);
        }
    }

    public void SetState(EState aState)
    {
        m_State = aState;
        SetAnimatorInteger(ANIM_PARAM_STATE, (int)m_State);
    }
}
