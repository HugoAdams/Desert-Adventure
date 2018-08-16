using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleMin : EnemyBase {

    ENEMYSTATE m_state;
    float m_lastWanderTime = -1;
    bool m_wanderRotate = false;
    float m_waitTime;

    float m_bounceTime = -1;
    float m_height = 0;
    public bool walkaround = true;
    //public float wanderRadius = 8;

    // Use this for initialization
    void Start () {

        if (walkaround)
        {
            m_state = ENEMYSTATE.IDLE;
        }
        else
        {
            m_state = ENEMYSTATE.WANDER;
        }
        //m_wanderCircle = wanderRadius;
        m_bounceTime = Time.time;
        m_startPos = transform.position;
        m_wanderTarget = m_startPos;
        m_moveSpeed = m_maxMoveSpeed;
        m_rotSpeed = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {

        switch (m_state)
        {
            case ENEMYSTATE.ATTACK:
                SwitchToWander();
                break;
            case ENEMYSTATE.IDLE:
                Idle();
                break;
            case ENEMYSTATE.WANDER:
                Wander();
                break;
            case ENEMYSTATE.DAMAGE:
                SwitchToWander();
                break;
            case ENEMYSTATE.DEATH:
                SwitchToWander();
                break;
            case ENEMYSTATE.SPECIAL:
                SwitchToWander();
                break;
            case ENEMYSTATE.NULL:
                SwitchToWander();
                break;

            default:
                break;
        }

        SetOnGround();

        if(IsTimerDone(m_bounceTime, 0.7f))
        {
            if(m_height > 0.05f)
            {
                m_height = 0.0f;
            }
            else
            {
                m_height = 0.1f;
            }
            m_bounceTime = Time.time;
        }
	}

    void SetOnGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, 0.8f, (int)m_groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f + m_height, transform.position.z);
        }
    }

    void SwitchToWander()
    {
        m_state = ENEMYSTATE.WANDER;
    }

    public override void OnDeath()
    {
        Debug.Log(name + "shouldnt be in this state");
    }

    public override void OnEnemyHit(int _damage, Transform _attacker)
    {
        Debug.Log(name + "shouldnt be in this state");
    }

    protected override void Attack()
    {
        Debug.Log(name + "shouldnt be in this state");
    }

    protected override void DamageStun()
    {
        Debug.Log(name + "shouldnt be in this state");
    }

    protected override void Idle()
    {
        if(walkaround == true)
        {
            return;
        }
        if (IsTimerDone(m_lastWanderTime, m_waitTime))
        {
            m_state = ENEMYSTATE.WANDER;
        }
    }

    protected override void Wander()
    {//similar to stone face wander
        if (m_wanderRotate == false)
        {
            Vector3 oldWander = m_wanderTarget;
            m_wanderTarget = PathWanderBasic(m_wanderTarget);

            if (oldWander != m_wanderTarget)
            {
                m_wanderRotate = true;
                m_lastWanderTime = Time.time;
                m_waitTime = Random.Range(2.4f, 5.0f);
                m_state = ENEMYSTATE.IDLE;
            }
        }

        if(m_wanderRotate == true)
        {
            m_wanderRotate = RotateToFace(m_wanderTarget);
        }
        else
        {
            PathSteering(PathSeek(m_wanderTarget));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // hit by player
        SoundEffectsPlayer.Instance.PlaySound("Guitar");
    }

}
