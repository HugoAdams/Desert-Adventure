using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusMin : EnemyBase
{
    ENEMYSTATE m_state;
    ENEMYSTATE m_enterDamageState;
    Animator m_anima;
    CactusColliders m_colliders;
    Renderer m_renderer;

    float m_targetTimer = 5;//seconds
    float m_targetAquiredTime = -1;
    float m_idleTime;
    float m_lastWanderTime = -1;
    float m_moveSpeedLocal;
    float m_attackTime = 0;

    float m_damageStartTime = 0;
    float m_deathStartTime = 0;

    void Start ()
    {
        m_state = ENEMYSTATE.WANDER;
        m_target = null;
        m_targetAquiredTime = Time.time;
        m_startPos = transform.position;

        m_moveSpeed = 0.6f * m_maxMoveSpeed;
        m_moveSpeedLocal = m_moveSpeed;

        m_anima = GetComponentInChildren<Animator>();
        m_colliders = GetComponentInChildren<CactusColliders>();
        m_colliders.m_spearTrigger.enabled = false;

        m_renderer = GetComponentInChildren<Renderer>();
        m_normalMat = m_renderer.material;
        m_currentHealth = m_MaxHealth;
    }

    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            OnEnemyHit(1, null);
        }*/

        switch (m_state)
        {
            case ENEMYSTATE.IDLE:
                Idle();
                break;
            case ENEMYSTATE.WANDER:
                m_moveSpeed = m_moveSpeedLocal;
                Wander();
                break;
            case ENEMYSTATE.ATTACK:
                m_moveSpeed = m_maxMoveSpeed;
                Attack();
                break;
            case ENEMYSTATE.DAMAGE:
                DamageStun();
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

        SetOnGround();
    }

    public override void OnDeath()
    {
        if (IsTimerDone(m_deathStartTime, 1.2f))
        {
            m_colliders.DisableAll();
            if (IsTimerDone(m_deathStartTime, 13.0f))
            {
                Destroy(gameObject);
            }
        }
    }

    public override void OnEnemyHit(int _damage, Transform _attacker)
    {
        if (m_state == ENEMYSTATE.DAMAGE)
        {//invincible while in damage
            //Debug.Log("nope");
            return;
        }

        Debug.Log(name + " has taken " + _damage + " damage");
        m_currentHealth -= _damage;

        if (m_currentHealth <= 0 && m_state != ENEMYSTATE.DEATH)
        {//die
            Debug.Log(name + " has died :-( ");
            m_anima.SetTrigger("StartDeath");
            m_state = ENEMYSTATE.DEATH;
            m_deathStartTime = Time.time;
        }
        else
        {
            m_damageStartTime = Time.time;
            m_enterDamageState = m_state;
            m_state = ENEMYSTATE.DAMAGE;
            m_anima.SetTrigger("StartDamage");
        }

        if (_attacker != null)
        {
            if (_attacker.GetComponent<PlayerController>())//check if transform has player controller
            {
                m_target = _attacker;
                m_targetAquiredTime = Time.time;
            }
        }
    }

    protected override void DamageStun()
    {
        int t = Mathf.RoundToInt((Time.time - m_damageStartTime) * 10);
        if(t % 2 == 0)
        {
            m_renderer.material = m_damageMat;
        }
        else
        {
            m_renderer.material = m_normalMat;
        }

        if(IsTimerDone(m_damageStartTime, 2.0f))
        {
            m_state = m_enterDamageState;
            m_renderer.material = m_normalMat;
        }
    }

    protected override void Idle()
    {
        //we dont move
        if(IsTimerDone(m_idleTime, 3.0f))
        {
            if(Random.Range(0,90) == 0)
            {
                m_state = ENEMYSTATE.WANDER;
                m_anima.SetBool("isMoving", true);
            }
        }

        if (Sight())
        {
            m_targetAquiredTime = Time.time;
            m_state = ENEMYSTATE.ATTACK;
        }

    }

    protected override void Attack()
    {
        //go go go
        if (m_target != null)
        {
            LookAt(m_target);

            if (Vector3.Distance(transform.position, m_target.position) < 2.2f)
            {
                if (IsTimerDone(m_attackTime, 1.5f))
                {
                    m_attackTime = Time.time;
                    m_anima.SetTrigger("StartAttack");
                }
                else
                {
                    if(m_anima.GetBool("isMoving") != false)
                    {
                        m_anima.SetBool("isMoving", false);
                    }
                }
            }
            else
            {
                PathSteering(PathSeek(m_target.position));

                if (m_anima.GetBool("isMoving") != true)
                {
                    m_anima.SetBool("isMoving", true);
                }
            }

            if(IsTimerDone(m_targetAquiredTime,m_targetTimer))
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
            if(Sight())
            {
                m_targetAquiredTime = Time.time;
            }
            else
            {
                m_state = ENEMYSTATE.WANDER;
                if (m_anima.GetBool("isMoving") != true)
                {
                    m_anima.SetBool("isMoving", true);
                }
            }
        }
    }

    protected override void Wander()
    {//use this for basic movement
        //and returning to start area
        Vector3 oldwander = m_wanderTarget;
        m_wanderTarget = PathWander(m_lastWanderTime, m_wanderTarget);

        if(oldwander != m_wanderTarget)
        {
            m_lastWanderTime = Time.time;
            if((Vector3.Distance(transform.position, m_startPos) < 8)&&(Random.Range(0,4) == 0))
            {
                m_state = ENEMYSTATE.IDLE;
                m_anima.SetBool("isMoving", false);
                m_idleTime = Time.time;
            }
        }
        PathSteering(PathSeek(m_wanderTarget));
        
        LookAt(m_wanderTarget);

        if (Sight())
        {
            m_targetAquiredTime = Time.time;
            m_state = ENEMYSTATE.ATTACK;
        }
    }

    void SetOnGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, 0.8f, (int)m_groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
        }
    }



    private void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.tag == "Player")
        {
            Debug.Log("hit");
            Rigidbody rbdy = GetComponent<Rigidbody>();
            rbdy.AddForce((transform.position - coll.transform.position) * 2200 * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
