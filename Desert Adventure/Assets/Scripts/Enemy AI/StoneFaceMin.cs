﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFaceMin : EnemyBase
{
    enum BIGSTATE
    {
        STANDING,
        FALLING,
        MOVING,
        ROTATING,
        STANDING_UP,
        NULL
    }

    ENEMYSTATE m_state;
    BIGSTATE m_bigState;
    Rigidbody m_rbdy;
    Animator m_anima;
    StoneFaceColliders m_colliders;

    float m_targetTimer = 5;//seconds
    float m_targetAquiredTime = -1;
    Vector3 fallStart = Vector3.zero;
    float fallStartTime = -1;
    float standUpTime = 0;
    float m_lastWanderTime = -1;
    bool m_wanderRotate = false;
    float m_deathStartTime = -1;

    float m_waitTime = 0;

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
        m_colliders = GetComponentInChildren<StoneFaceColliders>();
        m_currentHealth = m_MaxHealth;
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
            case ENEMYSTATE.DEATH:
                OnDeath();
                break;
            case ENEMYSTATE.NULL:
                Debug.Log(name + " in null state");
                break;
            default:
                Debug.Log(name + " in ??? state");
                break;
        }
        
        if(m_state != ENEMYSTATE.DEATH)
        {
        SetOnGround();
        //OnEnemyHit(400, transform);

        }
        
    }

    public override void OnDeath()
    {
        if (IsTimerDone(m_deathStartTime, 1.2f))
        {
            transform.position = transform.position + (Vector3.down * m_moveSpeed * Time.deltaTime);
            Debug.Log("falling");

            m_colliders.DisableAll();
            if (IsTimerDone(m_deathStartTime, 13.0f))
            {
                //delete self
                Destroy(gameObject);
            }
        }
    }
    public override void OnEnemyHit(int _damage, Transform _attacker)
    {
        m_currentHealth -= _damage;
        if(m_currentHealth <= 0 && m_state != ENEMYSTATE.DEATH)
        {
            Debug.Log(name + " has died :<");
            m_anima.SetTrigger("StartDeath");
            m_state = ENEMYSTATE.DEATH;
            m_deathStartTime = Time.time;
        }
    }

    protected override void Idle()
    {
        //Dont Move
        if (IsTimerDone(m_lastWanderTime, m_waitTime))
        {
            m_state = ENEMYSTATE.WANDER;
            m_anima.SetBool("isRunning", true);
        }

        if (Sight())
        {
            m_state = ENEMYSTATE.ATTACK;
            m_targetAquiredTime = Time.time;
        }
    }

    protected override void Attack()
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


    protected override void Wander()
    {
        //PathWander
        if (m_wanderRotate == false)//small optimise
        {
            Vector3 oldwander = m_wanderTarget;
            m_wanderTarget = PathWanderBasic(m_wanderTarget);


            if (oldwander != m_wanderTarget)
            {//new target
                m_wanderRotate = true;
                m_lastWanderTime = Time.time;

                m_waitTime = Random.Range(0.6f, 3.0f);
                m_anima.SetBool("isRunning", false);
                m_state = ENEMYSTATE.IDLE;
            }
        }


        
        if (m_wanderRotate == true)
        {
            m_wanderRotate = RotateToFace(m_wanderTarget);
        }
        else
        {
            PathSteering(PathSeek(m_wanderTarget));

        }
        
        
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
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f , transform.position.z);
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

    bool RotateToFace(Vector3 _v)
    {
        _v.y = transform.position.y;//should stop rotation on x and z;

        Vector3 targetDir = _v - transform.position;
        Vector3 newdir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * 2, 0.0f);

        transform.rotation = Quaternion.LookRotation(newdir);
        
        if(Vector3.Angle(transform.forward,targetDir) < 0.5f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}
