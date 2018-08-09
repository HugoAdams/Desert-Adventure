using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFaceMin : EnemyBase
{
    enum BIGSTATE
    {
        STANDING,
        FALLING,
        MOVING,
        ROLLING,
        STANDING_UP,
        NULL
    }
    ENEMYSTATE m_state;
    BIGSTATE m_bigState;
    Rigidbody m_rbdy;

    float m_targetTimer = 5;//seconds
    float m_targetAquiredTime = -1;
    Vector3 fallStart = Vector3.zero;
    float fallStartTime = -1;
    float standUpTime = 0;
    float m_lastWanderTime = -1;
    Animator m_anima;

    void Start ()
    {
        m_state = ENEMYSTATE.WANDER;
        m_bigState = BIGSTATE.STANDING;
        m_target = null;
        m_startPos = transform.position;
        m_targetAquiredTime = Time.time;

        m_moveSpeed = m_maxMoveSpeed;
        m_rbdy = GetComponent<Rigidbody>();
        m_anima = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        switch (m_state)
        {
            case ENEMYSTATE.IDLE:
                Idle();
                break;
            case ENEMYSTATE.WANDER:
                Wander();
                break;
            case ENEMYSTATE.ATTACK:
                Attack();
                break;
            case ENEMYSTATE.NULL:
                Debug.Log(name + " in null state");
                break;
            default:
                Debug.Log(name + " in ??? state");
                break;
        }
        
        SetOnGround();
        
    }

    public override void OnDeath()
    {
        Debug.Log(name + " has died :<");
    }
    public override void OnEnemyHit(int _damage, Transform _attacker)
    {
        m_currentHealth -= _damage;
        if(m_currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public override void Idle()
    {
        //Dont Move
    }

    public override void Attack()
    {
        if(m_bigState == BIGSTATE.FALLING)
        {
            Fall();
        }
        else if(m_bigState == BIGSTATE.STANDING_UP)
        {
            Stand();
        }

        if(m_target != null)
        {
            if (m_bigState != BIGSTATE.STANDING_UP && m_bigState != BIGSTATE.FALLING)
            {
                if (Distance2D(transform.position, m_target.position) < 4.2f && IsTimerDone(standUpTime,3))
                {
                    Fall();
                    return;
                }

                PathSteering(PathSeek(m_target.position));//move logic
                LookAt(m_target);
            }


            if(IsTimerDone(m_targetAquiredTime, m_targetTimer))
            {
                //been time check if you can still see them

                if (Sight())
                {
                    m_targetAquiredTime = Time.time;
                }

            }
        }
        else
        {
            if(Sight() == false)
            {
                m_state = ENEMYSTATE.WANDER;
            }
        }
    }


    public override void Wander()
    {
        //PathWander
        Vector3 oldwander = m_wanderTarget;
        m_wanderTarget = PathWander(m_lastWanderTime, m_wanderTarget);
        if (oldwander != m_wanderTarget)
        {
            m_lastWanderTime = Time.time;
        }
            PathSteering(PathSeek(m_wanderTarget));
            LookAt(m_wanderTarget);
        
        
        if(Sight())
        {
            m_state = ENEMYSTATE.ATTACK;
            m_targetAquiredTime = Time.time;
        }
    }

    void SetOnGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, 4.0f, (int)m_groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y+0.2f , transform.position.z);
            if(m_bigState == BIGSTATE.STANDING)
            {
               m_rbdy.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }

    void Fall()
    {
        if(fallStartTime == -1)
        {
            m_bigState = BIGSTATE.FALLING;
            fallStart = transform.eulerAngles;
            fallStartTime = Time.time;            
        }

        float t = (Time.time - fallStartTime) * 2;
        //Debug.Log(t);
        Vector3 v = Vector3.Lerp(fallStart, new Vector3(90, fallStart.y, 0), t);
        transform.eulerAngles = v;

        if (IsTimerDone(fallStartTime, 6)) 
        {
            fallStartTime = -1;
            m_bigState = BIGSTATE.STANDING_UP;
        }
    }

    void Stand()
    {
        if(fallStartTime == -1)
        {
            fallStartTime = Time.time;
            fallStart = transform.eulerAngles;
        }

        float t = (Time.time - fallStartTime) * 3;
        Vector3 v = Vector3.Lerp(fallStart, new Vector3(0, fallStart.y, 0), t);
        transform.eulerAngles = v;

        if (t >= 1)
        {
            fallStartTime = -1;
            m_bigState = BIGSTATE.STANDING;
            standUpTime = Time.time;
        }
    }
}
