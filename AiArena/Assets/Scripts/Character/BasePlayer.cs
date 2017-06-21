using System.Collections;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    protected enum EMoveDir
    {
        Forward,
        Backward,
    }

    protected enum ETurnDir
    {
        Left,
        Right,
    }
    
    [SerializeField] protected PlayerData m_PlayerData;
    [SerializeField] protected Weapon m_Weapon;
    [SerializeField] private GameObject m_Shield;

    protected Transform m_Transform;
    protected StateMachine m_StateMachine;

    private int m_Id;
    private Ball m_Ball;

    private int m_Life;

    private Coroutine m_StunRoutine;
    private Coroutine m_PowerUpRoutine;

    private Vector3 m_Movement;
    private Vector3 m_Rotation;
    
    protected virtual void Awake()
    {
        m_Transform = transform;
    }

    protected virtual void Start()
    {
        InitStateMachine();
    }
    
    /// <summary>
    /// Do not touch
    /// </summary>
    public void Init(int aId)
    {
        m_Id = aId;

        CalculateStats();
        
        m_Weapon.Init(aId, this);

        GetComponentInChildren<SpriteRenderer>().color = GameManager.Instance.Data.PlayerColorList[aId];

        m_Shield.SetActive(false);

        GameManager.Instance.OnObjectiveCompleteCallback += OnObjectiveCompleteCallback;
    }

    /// <summary>
    /// Do not touch
    /// </summary>
    public void OnUpdate()
    {
        m_StateMachine.SMUpdate();
        
        ComputeMovements();
    }

    protected virtual void InitStateMachine()
    {
        m_StateMachine = new StateMachine();
    }

    private void CalculateStats()
    {
        var gameData = GameManager.Instance.Data;

        m_Life = gameData.BaseLife + (int)GetSkillValue(gameData.MaxSkillBonusLife, m_PlayerData.ExtraLife);

        ShieldCooldown = gameData.ShieldCooldown;
        MoveSpeed = gameData.MoveSpeed + GetSkillValue(gameData.MaxSkillBonusMove, m_PlayerData.FasterMove);
        TurnSpeed = gameData.TurnSpeed + GetSkillValue(gameData.MaxSkillBonusTurn, m_PlayerData.FasterTurn);
        ShieldDuration = gameData.ShieldDuration + GetSkillValue(gameData.MaxSkillBonusShield, m_PlayerData.ImprovedShield);
        WeaponLength = gameData.WeaponLength + GetSkillValue(gameData.MaxSkillWeaponLength, m_PlayerData.ExtraWeaponLength);
        StunDuration = gameData.StunDuration + GetSkillValue(gameData.MaxSkillBonusExtraStun, m_PlayerData.ExtraStun);
    }
    
    protected void Move(EMoveDir aDir)
    {
        if (IsStunned)
            return;

        int dir = aDir == EMoveDir.Forward ? 1 : -1;

        float speed = MoveSpeed;
        if (HasPowerUp)
        {
            speed *= GameManager.Instance.Data.PowerUpSpeedMultiplier;
        }

        m_Movement = Vector3.right * speed * dir * Time.deltaTime;
    }

    protected void Turn(ETurnDir aDir)
    {
        if (IsStunned)
            return;

        int dir = aDir == ETurnDir.Left ? 1 : -1;

        float speed = TurnSpeed;
        if (HasPowerUp)
        {
            speed *= GameManager.Instance.Data.PowerUpSpeedMultiplier;
        }

        m_Rotation = Vector3.forward * speed * dir * Time.deltaTime;
    }

    protected void ActivateShield()
    {
        if (IsShieldInCooldown)
            return;

        RemoveStun();

        StartCoroutine(ShieldRoutine(ShieldDuration));
        StartCoroutine(ShieldCooldownRoutine(ShieldCooldown));
    }

    private void ComputeMovements()
    {
        m_Transform.Translate(m_Movement);
        m_Transform.Rotate(m_Rotation);

        m_Movement = Vector3.zero;
        m_Rotation = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D aCollider)
    {
        if (IsStunned)
            return;

        if (aCollider.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            OnCollisionBall(aCollider);
        }
        else if (aCollider.gameObject.layer == LayerMask.NameToLayer("Goal") && HasBall)
        {
            if (GameManager.Instance.GetGoalPos(Id) == aCollider.transform.position)
            {
                OnCollisionGoalWithBall();
            }
        }
        else if (aCollider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            OnCollisionWeapon(aCollider);
        }
        else if (aCollider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
        {
            OnCollisionPowerUp(aCollider);
        }
    }
    
    private void OnTriggerStay2D(Collider2D aCollider)
    {
        if (IsStunned)
            return;

        if (aCollider.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            OnCollisionWeapon(aCollider);
        }
    }

    private void OnCollisionGoalWithBall()
    {
        ObjectiveCount++;

        m_Ball.Reset();
        m_Ball = null;

        GameManager.Instance.ObjectiveComplete(Id);
    }     

    private void OnCollisionBall(Collider2D aCollider)
    {
        var ball = aCollider.GetComponent<Ball>();

        // Safe check
        if (!ball.IsPickedUp)
        {
            m_Ball = ball;
            m_Ball.PickUp(m_Transform, m_Id);
        }
    }

    private void OnCollisionWeapon(Collider2D aCollider)
    {
        if (IsShieldUp)
            return;

        Weapon weapon = aCollider.GetComponentInParent<Weapon>();

        if (weapon != null && weapon != m_Weapon)
        {
            OnHit(weapon);
        }
    }

    private void OnCollisionPowerUp(Collider2D aCollider)
    {
        if (m_PowerUpRoutine != null)
        {
            StopCoroutine(m_PowerUpRoutine);
        }

        m_PowerUpRoutine = StartCoroutine(PowerUpRoutine(GameManager.Instance.Data.PowerUpDuration));
        PowerUpManager.Instance.RemovePowerUp(aCollider.gameObject.GetComponent<PowerUp>());
    }

    private void OnHit(Weapon aWeapon)
    {
        if (HasBall)
        {
            m_Ball.Drop();
            m_Ball = null;
        }

        m_Life--;
        if (m_Life <= 0)
        {
            Die();
        }
        else
        {
            m_StunRoutine = StartCoroutine(StunRoutine(aWeapon.Player.StunDuration));
        }

        HUD.Instance.UpdatePlayerLife(m_Id, m_Life);

        OnHitCallback(aWeapon.Player.Id);
    }
    
    /// <param name="aPlayerId">=This is the Id of the player that hit you</param>
    protected virtual void OnHitCallback(int aPlayerId) { }
    /// <param name="aPlayerId">=This is the Id of the player that just completed an objective</param>
    protected virtual void OnObjectiveCompleteCallback(int aPlayerId) { }

    private void Die()
    {
        // Remove player from the map
        gameObject.SetActive(false);
    }

    private void RemoveStun()
    {
        if (m_StunRoutine == null)
        {
            return;
        }

        StopCoroutine(m_StunRoutine);

        IsStunned = false;
        m_Weapon.SetActive(true);
    }
    
    private IEnumerator StunRoutine(float aDuration)
    {
        IsStunned = true;
        m_Weapon.SetActive(false);

        yield return new WaitForSeconds(aDuration);

        m_Weapon.SetActive(true);
        IsStunned = false;
    }

    private IEnumerator PowerUpRoutine(float aDuration)
    {
        HasPowerUp = true;
        yield return new WaitForSeconds(aDuration);
        HasPowerUp = false;
    }

    private IEnumerator ShieldRoutine(float aDuration)
    {
        IsShieldUp = true;        
        m_Shield.SetActive(true);

        yield return new WaitForSeconds(aDuration);

        m_Shield.SetActive(false);
        IsShieldUp = false;
    }

    private IEnumerator ShieldCooldownRoutine(float aDuration)
    {
        IsShieldInCooldown = true;
        yield return new WaitForSeconds(aDuration);
        IsShieldInCooldown = false;
    }

    public bool HasBall
    {
        get { return m_Ball != null; }
    }

    public int ObjectiveCount { get; private set; }

    public bool IsStunned { get; private set; }
    public bool IsShieldUp { get; private set; }
    public bool IsShieldInCooldown { get; private set; }
    public bool HasPowerUp { get; private set; }

    private float GetSkillValue(float aMaxValue, int aPointsAllocated)
    {
        return aMaxValue * ((float)aPointsAllocated / (float)GameData.MAX_SKILL_LEVEL);
    }

    public int Id
    {
        get { return m_Id; }
    }

    public int Life
    {
        get { return m_Life; }
    }

    public string PlayerName
    {
        get { return m_PlayerData.PlayerName; }
    }

    public float ShieldCooldown { get; private set; }
    public float MoveSpeed  { get; private set; }
    public float TurnSpeed  { get; private set; }
    public float ShieldDuration { get; private set; }
    public float WeaponLength { get; private set; }
    public float StunDuration { get; private set; }

    public PlayerData Data
    {
        get
        {
            return m_PlayerData;
        }
    }
}
