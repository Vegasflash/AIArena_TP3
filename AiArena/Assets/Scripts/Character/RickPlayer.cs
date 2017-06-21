using UnityEngine;
using System.Collections.Generic;

public class RickPlayer : BasePlayer
{
    public enum EState
    {
        Initialize,
        Idle,
        Move,
        MoveTowardsGoal,
        MoveTowardsPowerup,
        MoveTowardsBall,
        MoveToAttackPlayer,
        MoveToInterceptCarrier,
        MoveTointerceptPlayer
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();

        m_StateMachine.AddState(EState.Initialize, StateMachine.TypeActionState.OnEnter, OnInitialize);
        m_StateMachine.AddState(EState.Idle, StateMachine.TypeActionState.Update, OnIdleUpdate);
        m_StateMachine.AddState(EState.Move, StateMachine.TypeActionState.Update, OnMoveUpdate);
        m_StateMachine.AddState(EState.MoveTowardsGoal, StateMachine.TypeActionState.Update, OnMoveTowardsGoalUpdate);
        m_StateMachine.AddState(EState.MoveTowardsBall, StateMachine.TypeActionState.Update, OnMoveTowardsBallUpdate);
        m_StateMachine.AddState(EState.MoveTowardsPowerup, StateMachine.TypeActionState.Update, OnMoveTowardsPowerUpUpdate);
        m_StateMachine.SetState(EState.Initialize);
    }

    private float m_Counter;
    int number_Of_Players;
    int number_Of_PowerUps;
    int powerupindex;
    int playerindex;
    bool pickupNearby;

    // Power_Up positions
    Vector3 first_Power_Up_Pos;
    Vector3 second_Power_Up_Pos;
    Vector3 third_Power_Up_Pos;

    // Player positions
    Vector3 first_Player_Pos;
    Vector3 second_Player_Pos;
    Vector3 third_Player_Pos;

    // Ball Position
    Vector3 ballPos;
    Vector3 goalPos;
    Vector3 playerTarget;
    Vector3 powerUpTarget;

    Vector3 current_PlayerPos;
    
    private void OnInitialize()
    {    
        number_Of_Players = GameManager.Instance.GetAllPlayerPos().Count;
        powerupindex = -1;
        m_StateMachine.SetState(EState.Move);
    }

    private void OnMoveUpdate()
    {
        UpdateBallPos();
        // Movement
        if (!HasBall)
        {
            m_StateMachine.SetState(EState.MoveTowardsBall);
        }
        else
            m_StateMachine.SetState(EState.MoveTowardsGoal);
    }

    private void MoveToBall()
    {
        Vector3 front = transform.right;

        ballPos = GameManager.Instance.GetBallPos();
        Vector3 cross = Vector3.Cross(front, (ballPos - transform.position));

        float distanceSqr = (transform.position - ballPos).sqrMagnitude;
        float rangeSqr = 2.0f;

        if (Vector3.Dot(front, (ballPos - transform.position)) < 0)
        {
            if (distanceSqr > rangeSqr || Mathf.Abs(cross.z) < 0.1f)
            {
                Move(EMoveDir.Backward);
            }
            if (cross.z > 0)
            {
                Turn(ETurnDir.Right);
            }
            if (cross.z < 0)
            {
                Turn(ETurnDir.Left);
            }
        }

        if (Vector3.Dot(front, (ballPos - transform.position)) > 0)
        {
            if (distanceSqr > rangeSqr || Mathf.Abs(cross.z) < 0.1f)
                Move(EMoveDir.Forward);

            if (cross.z > 0)
            {
                Turn(ETurnDir.Left);
            }
            if (cross.z < 0)
            {
                Turn(ETurnDir.Right);
            }
        }
    }

    private void MoveToPowerUp()
    {
        Vector3 front = transform.right;
        Vector3 cross = Vector3.Cross(front, (powerUpTarget - transform.position));

        float distanceSqr = (transform.position - powerUpTarget).sqrMagnitude;
        float rangeSqr = 2.0f;

        if (Vector3.Dot(front, (powerUpTarget - transform.position)) < 0)
        {

            if (distanceSqr > rangeSqr || Mathf.Abs(cross.z) < 0.1f)
                Move(EMoveDir.Backward);

            if (cross.z > 0)
            {
                Turn(ETurnDir.Right);
            }
            if (cross.z < 0)
            {
                Turn(ETurnDir.Left);
            }
        }

        if (Vector3.Dot(front, (powerUpTarget - transform.position)) > 0)
        {
            if (distanceSqr > rangeSqr || Mathf.Abs(cross.z) < 0.1f)
            {
                Move(EMoveDir.Forward);
            }

            if (cross.z > 0)
            {
                Turn(ETurnDir.Left);
            }
            if (cross.z < 0)
            {
                Turn(ETurnDir.Right);
            }
        }
    }

    private void MoveToGoal()
    {
        Vector3 front = transform.right;

        goalPos = GameManager.Instance.GetGoalPos(Id);
        Vector3 cross = Vector3.Cross(front, (goalPos - transform.position));
        float distanceSqr = (transform.position - powerUpTarget).sqrMagnitude;
        float rangeSqr = 2.0f;

        if (Vector3.Dot(front, (goalPos - transform.position)) < 0)
        {
            if (distanceSqr > rangeSqr || Mathf.Abs(cross.z) < 0.1f)
            {
                Move(EMoveDir.Backward);
            }
            if (cross.z > 0)
            {
                Turn(ETurnDir.Right);
            }
            if (cross.z < 0)
            {
                Turn(ETurnDir.Left);
            }
        }

        if (Vector3.Dot(front, (goalPos - transform.position)) > 0)
        {
            if (distanceSqr > rangeSqr || Mathf.Abs(cross.z) < 0.1f)
            {
                Move(EMoveDir.Forward);
            }
            if (cross.z > 0)
            {
                Turn(ETurnDir.Left);
            }
            if (cross.z < 0)
            {
                Turn(ETurnDir.Right);
            }
        }
    }




    private void OnIdleUpdate()
    {
        UpdateBallPos();

        MoveToBall();

        m_Counter += Time.deltaTime;

    }


    private void OnMoveTowardsGoalUpdate()
    {
        UpdateBallPos();

        if (DistanceToPlayer() <= 0.70f)
        {
            ActivateShield();
        }

        if (HasBall)
        {
            if (DistanceToGoal() - 2.0f <= DistanceToPowerUp())
            {
                MoveToGoal();
            }
            else
            {
                m_StateMachine.SetState(EState.MoveTowardsPowerup);
            }
        }
        else
        {
            if(DistanceToBall() <= DistanceToPowerUp())
            {
                m_StateMachine.SetState(EState.MoveTowardsBall);
            }
            else
            {
                m_StateMachine.SetState(EState.MoveTowardsPowerup);
            }
        }
    }

    private void OnMoveTowardsBallUpdate()
    {
        UpdateBallPos();

        if (DistanceToPlayer() <= 0.70f)
        {
            ActivateShield();
        }

        if (!HasBall)
        {
            MoveToBall();
            if (DistanceToPowerUp() - 0.5f <= DistanceToBall())
            {
                m_StateMachine.SetState(EState.MoveTowardsPowerup);
            }            
        }
        else
        {
            m_StateMachine.SetState(EState.MoveTowardsGoal);
        }
    }

    private void OnMoveTowardsPowerUpUpdate()
    {
        UpdateBallPos();
        if(DistanceToPlayer() <= 0.70f)
        {
            ActivateShield();
        }

        if(!HasBall)
        {
            if (DistanceToBall() + 0.5f <= DistanceToPowerUp())
            {
                m_StateMachine.SetState(EState.MoveTowardsBall);
            }
            else
            {
                if (GameManager.Instance.GetPowerUpPositions().Count != 0)
                    MoveToPowerUp();
            }
        }
        else
        {
            m_StateMachine.SetState(EState.MoveTowardsGoal);
        }
    }

    private float DistanceToBall()
    {
        float distance = 0;            
        distance = Vector3.Distance(transform.position, ballPos);      
        return distance;
    }

    private float DistanceToGoal()
    {
        float distance = 0;

        distance = Vector3.Distance(transform.position, GameManager.Instance.GetGoalPos(Id));
        
        return distance;
    }

    private float DistanceToPlayer()
    {
        float distance = float.MaxValue;
        float minDistance = float.MaxValue;
        float offset = 0.35f;
        for (int i = 0; i < GameManager.Instance.GetAllPlayerPos().Count; i++)
        {
            if (Id == i)
                continue;

            if (GameManager.Instance.GetPlayerLife(i) != 0)
            {
                distance = Vector3.Distance(transform.position,
                                            GameManager.Instance.GetAllPlayerPos()[i] + 
                                            GameManager.Instance.GetPlayerDirection(i) *
                                            (GameManager.Instance.GetPlayerWeaponLength(i) + offset));
            }
            if (distance <= minDistance)
            {
                playerindex = i;
                minDistance = distance;
                playerTarget = GameManager.Instance.GetAllPlayerPos()[playerindex];
            }          
        }
        return distance;
    }

    private float DistanceToPowerUp()
    {
        float distance = float .MaxValue;
        float minDistance = float.MaxValue;
        
        for (int i = 0; i < GameManager.Instance.GetPowerUpPositions().Count; i++)
        {
            distance = Vector3.Distance(transform.position, GameManager.Instance.GetPowerUpPositions()[i]);
            
            if (distance <= minDistance)
            {
                powerupindex = i;
                minDistance = distance;
                powerUpTarget = GameManager.Instance.GetPowerUpPositions()[powerupindex];
            }
        }
        return distance;
    }

    // ASSIGN THIS UPDATE TO ALL UPDATE STATES TO ACTIVELY UPDATE THE BALL POSITION ON THE MAP
    private Vector2 UpdateBallPos()
    {
        ballPos = GameManager.Instance.GetBallPos();
        if (ballPos != null)
            return GameManager.Instance.GetBallPos();
        else
            return Vector2.zero;
    }


    protected override void OnHitCallback(int aPlayerId)
    {
        LastPlayerThatHitMe = aPlayerId;
    }

    private int LastPlayerThatHitMe;

}
