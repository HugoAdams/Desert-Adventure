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
        NULL
    }
    ENEMYSTATE m_state;
    BIGSTATE m_bigState;
    Rigidbody m_rbdy;

    float m_targetTimer = 5;//seconds
    float m_targetAquiredTime = -1;
    CharacterController chara;

    void Start ()
    {
        m_state = ENEMYSTATE.WANDER;
        m_bigState = BIGSTATE.STANDING;
        m_target = null;
        m_startPos = transform.position;
        m_targetAquiredTime = Time.time;

        m_moveSpeed = m_maxMoveSpeed;
        chara = GetComponent<CharacterController>();
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

    }

    public override void Attack()
    {
        if(m_target != null)
        {
            PathSteering(PathSeek(m_target.position));


            if (Time.time - m_targetAquiredTime >= m_targetTimer)
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
        //chara.SimpleMove(Vector3.right  * m_moveSpeed);
        chara.SimpleMove(transform.forward * m_maxMoveSpeed);
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.y + 40 * Time.deltaTime, 0);
        Debug.Log(chara.isGrounded);
        
        if(Sight())
        {
            //m_state = ENEMYSTATE.ATTACK;
        }
    }

    void SetOnGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, 4.0f, (int)m_groundLayer))
        {
           // transform.position = new Vector3(transform.position.x, hit.point.y + 3.3f, transform.position.z);
            if(m_bigState == BIGSTATE.STANDING)
            {
                //m_rbdy.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }

}
