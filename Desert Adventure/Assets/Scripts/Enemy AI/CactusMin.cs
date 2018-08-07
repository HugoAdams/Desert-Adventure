using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusMin : EnemyBase
{
    ENEMYSTATE m_state;

    float m_targetTimer = 5;//seconds
    float m_targetAquiredTime = -1;
    float m_idleTime;
    float m_lastWanderTime = -1;
    float m_moveSpeedLocal;

    void Start ()
    {
        m_state = ENEMYSTATE.WANDER;
        m_target = null;
        m_targetAquiredTime = Time.time;
        m_startPos = transform.position;

        m_moveSpeed = 0.6f * m_maxMoveSpeed;
        m_moveSpeedLocal = m_moveSpeed;
    }

    void Update()
    {
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
        Debug.Log(name + " has died :-( ");
        Destroy(this);
    }

    public override void OnEnemyHit(int _damage, Transform _attacker)
    {
        Debug.Log(name + " has taken " + _damage + " damage");
        m_currentHealth -= _damage;

        if(m_currentHealth <= 0)
        {
            //die
            OnDeath();
        }

        if(true)//not done //check if transform has player controller
        {
            m_target = _attacker;
            m_targetAquiredTime = Time.time;
        }
    }

    public override void Idle()
    {
        //we dont move
        if(Time.time - m_idleTime >= 3.0f)
        {
            if(Random.Range(0,90) == 0)
            {
                m_state = ENEMYSTATE.WANDER;   
            }
        }

        if (Sight())
        {
            m_targetAquiredTime = Time.time;
            m_state = ENEMYSTATE.ATTACK;
        }

    }

    public override void Attack()
    {
        //go go go
        if (m_target != null)
        {
            LookAt(m_target);

            if (Vector3.Distance(transform.position, m_target.position) < 0.5f)
            {
                //attck
               // Rigidbody rbdy = GetComponent<Rigidbody>();
               // rbdy.AddForce((transform.position - m_target.transform.position) * 1500 * Time.deltaTime, ForceMode.Impulse);
            }
            else
            {
                PathSteering(PathSeek(m_target.position));
            }

            if(Time.time - m_targetAquiredTime >= m_targetTimer)
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
            }
        }
    }

    public override void Wander()
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
                m_idleTime = Time.time;
            }
        }
        PathSteering(PathSeek(m_wanderTarget));
        
        LookAt(transform.position + m_curVel);

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

    void LookAt(Transform _t)
    {
        transform.LookAt(_t);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    void LookAt(Vector3 _v3)
    {
        transform.LookAt(_v3);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
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
