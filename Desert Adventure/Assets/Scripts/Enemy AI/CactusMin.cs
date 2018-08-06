using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusMin : EnemyBase
{
    ENEMYSTATE m_state;

    float m_targetTimer = 12;//seconds
    float m_targetAquiredTime = -1;

	void Start ()
    {
        m_state = ENEMYSTATE.IDLE;
        m_target = null;
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
    }

    public override void Attack()
    {
        //go go go
        if (m_target != null)
        {
            Vector3.MoveTowards(transform.position, m_target.position, m_moveSpeed * Time.deltaTime);
            if(Time.time - m_targetAquiredTime >= m_targetTimer)
            {
                //been time check if you can still see them
                Sight();
            }
        }
    }

    public override void Wander()
    {//use this for basic movement

    }

    void SetOnGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, 100, (int)m_groundLayer);
        transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

    }
}
