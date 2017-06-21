using UnityEngine;
using System.Collections.Generic;
using System;

public class StateMachine 
{
	
	#region PUBLIC members
	
	/// <summary>
	/// Type action state for each state logic
	/// </summary>
	public enum TypeActionState
	{
		None		= -1,
		OnEnter		= 0,
		OnExit		= 1,
		Update		= 2,
		LateUpdate	= 3,
		FixedUpdate	= 4,
		Count
	}

	#endregion

	#region PRIVATE members	
	
	/// <summary>
	/// Acces info state -> state Composition
	/// </summary>
	private Dictionary<int, StateCompo> 		m_DicState 					= new Dictionary<int, StateCompo>();
	
	private int 								m_CurrentState 				= -1;
    private int                                 m_ChangeState               = -1;
	private bool 								m_SMInit		 			= false;
    private bool                                m_SetStateNextFrame         = false;
	
	private Action 								m_TmpCall					= null;

	#endregion
	
	#region ACCESSORS
	
	public int CurrentState
	{
		get {return m_CurrentState;}
	}
	
	public bool SMInit
	{
		get{return m_SMInit;}
	}

    public bool SetStateNextFrame
    {
        set { m_SetStateNextFrame = value; }
        get { return m_SetStateNextFrame; }
    }
	
	#endregion	
	
	#region MONO METHODS	
	
	protected virtual void OnDestroy()
	{
		m_DicState 	= null;
		m_TmpCall	= null;
	}

	#endregion

    #region INIT STATE MACHINE

    /// <summary>
    /// Init this State Machine.
    /// </summary>
    protected virtual void Init()
    {
        if (m_DicState != null)
        {
            m_DicState.Clear();
        }
        m_SMInit = false;
        m_CurrentState = -1;
        m_ChangeState = -1;
    }

    /// <summary>
    /// Adds the state.
    /// </summary>
    /// <returns>
    /// Bool -> State correctly Add
    /// </returns>
    /// <param name='aNewStates'>
    /// Int ref new state Add
    /// </param>
    public bool AddStates(params System.Enum[] aNewStates)
    {
        return AddStateInternal(aNewStates);
    }

    /// <summary>
    /// AddState
    /// </summary>
    /// <param name="aState"></param>
    /// <param name="aType"></param>
    /// <param name="aAction"></param>
    /// <returns></returns>
    public bool AddState(System.Enum aState, TypeActionState aType, Action aAction )
    {
        if (!m_DicState.ContainsKey(Convert.ToInt32(aState)))
        {
            AddStateInternal(aState);           
        }
        m_DicState[Convert.ToInt32(aState)].m_Action[(int)aType] = aAction;
        return true;
    }

    /// <summary>
    /// AddStates
    /// </summary>
    /// <param name="aState"></param>
    /// <param name="aOnEnter"></param>
    /// <param name="aOnExit"></param>
    /// <param name="aUpdate"></param>
    /// <param name="aLateUpdate"></param>
    /// <param name="aFixedUpdate"></param>
    /// <returns></returns>
    public bool AddStates(System.Enum aState, Action aOnEnter, Action aOnExit, Action aUpdate, Action aLateUpdate, Action aFixedUpdate)
    {
        Debug.LogWarning("Use AddState( System.Enum aState,  TypeActionState aType , Action aAction) ");
        bool complete = true;
        complete = complete && AddState(aState,TypeActionState.OnEnter, aOnEnter);
        complete = complete && AddState(aState, TypeActionState.OnExit, aOnExit);
        complete = complete && AddState(aState, TypeActionState.Update, aUpdate);
        complete = complete && AddState(aState, TypeActionState.LateUpdate, aLateUpdate);
        complete = complete && AddState(aState, TypeActionState.FixedUpdate, aFixedUpdate);       
        return complete;
    }   

    /// <summary>
    /// AddStateInternal
    /// </summary>
    /// <param name="aNewStates"></param>
    /// <returns></returns>
    private bool AddStateInternal(params System.Enum[] aNewStates )
    {
        bool complete = true;

        foreach (System.Enum state in aNewStates)
        {
            if (!AddStateProcess(state))
            {
                complete = false;
            }
        }

        return complete;
    }

    /// <summary>
    /// Adds the state.
    /// </summary>
    /// <returns>
    /// Bool -> State correctly Add
    /// </returns>
    /// <param name='aNewState'>
    /// Int ref new state Add
    /// </param>
    private bool AddStateProcess(System.Enum aNewState)
    {
        if (!m_DicState.ContainsKey(Convert.ToInt32(aNewState)))
        {
            StateCompo newCompo = new StateCompo();
            m_DicState.Add(Convert.ToInt32(aNewState), newCompo);
            return true;
        }
        else
        {
            Debug.LogWarning("[IGLOO] STATE MACHINE: State " + aNewState + " already exist !!!");
            return false;
        }
    }

    #endregion

    #region ACTIVE MECANIQUE METHODES	
	
    /// <summary>
    /// SetState
    /// </summary>
    /// <param name="aNewState"></param>
    /// <returns></returns>
    public bool SetState(System.Enum aNewState)
    {
        return SetState(Convert.ToInt32(aNewState));
    }

	/// <summary>
	/// Sets the state.
	/// </summary>
	/// <returns>
	/// Bool Action Complete.
	/// </returns>
	/// <param name='aNewState'>
	/// New state to change
	/// </param>
	public bool SetState(int aNewState)
	{
		if(m_DicState.ContainsKey(aNewState))			
		{
			if(aNewState != CurrentState)
			{
                m_ChangeState = aNewState;
                if(!m_SetStateNextFrame) ChangeState();
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
            Debug.LogError("[IGLOO] The state machine doesn't contains this state : " + aNewState);
			return false;
		}
	}

    /// <summary>
    /// CompareState
    /// </summary>
    /// <param name="aState"></param>
    /// <returns></returns>
    public bool CompareState(System.Enum aState)
    {
        return (CurrentState == Convert.ToInt32(aState));
    }

    /// <summary>
    /// ChangeState
    /// </summary>
    private void ChangeState()
    {
        CallUnloadState();
        m_CurrentState = m_ChangeState;

        CallLoadState();
        m_SMInit = true;
    }

    /// <summary>
    /// Calls the state of the load.
    /// </summary>
    private void CallLoadState()
    {
        CallActionGenerique(TypeActionState.OnEnter);
    }

    /// <summary>
    /// Calls the state of the unload.
    /// </summary>
    private void CallUnloadState()
    {
        if (SMInit)
        {
            CallActionGenerique(TypeActionState.OnExit);
        }
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    public void SMUpdate()
    {
        if (m_SetStateNextFrame) ChangeState();
        CallActionGenerique(TypeActionState.Update);
    }

    /// <summary>
    /// Fixeds the update.
    /// </summary>
    public void SMFixedUpdate()
    {
        CallActionGenerique(TypeActionState.FixedUpdate);
    }

    /// <summary>
    /// Lates the update.
    /// </summary>
    public void SMLateUpdate()
    {
        CallActionGenerique(TypeActionState.LateUpdate);
    }

    /// <summary>
    /// Calls the action generique.
    /// </summary>
    /// <param name='aAction'>
    /// TypeActionState.
    /// </param>
    private void CallActionGenerique(TypeActionState aAction)
    {
        if (CurrentState >= 0)
        {
            m_TmpCall = m_DicState[CurrentState].m_Action[(int)aAction];

            if (m_TmpCall != null)
            {
                m_TmpCall();
            }
        }
    }	

	#endregion
    
    #region OBSOLETE METHODS TO DELETE

    /// <summary>
    /// State compo -> Public class for each state we associate methode for each step state
    /// </summary>
    public class StateCompo
    {
        /// <summary>
        /// Callback for each step
        /// </summary>
        public Action[] m_Action = null;

        public StateCompo()
        {
            m_Action = new Action[(int)TypeActionState.Count];
        }

        public StateCompo(Action aOnEnter, Action aOnExit, Action aUpdate, Action aLateUpdate, Action aFixedUpdate)
        {
            m_Action = new Action[(int)TypeActionState.Count];

            m_Action[(int)TypeActionState.OnEnter] = aOnEnter;
            m_Action[(int)TypeActionState.OnExit] = aOnExit;
            m_Action[(int)TypeActionState.Update] = aUpdate;
            m_Action[(int)TypeActionState.LateUpdate] = aLateUpdate;
            m_Action[(int)TypeActionState.FixedUpdate] = aFixedUpdate;
        }
    }

    #endregion
}
