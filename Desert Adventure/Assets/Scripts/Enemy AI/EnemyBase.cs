using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected enum ENEMYSTATE
    {
        IDLE,
        WANDER,
        ATTACK,
        DEATH,
        NULL,
        DAMAGE
    }

    float  m_Angle = 0;
    protected int m_currentHealth;
    [Range(1, 40)] public int m_MaxHealth = 10;
    [Range(1, 100)] public float m_maxMoveSpeed = 10;
    protected float m_moveSpeed = 5;

    [Header("Vision Stuff")]
    [Range(10, 180)] public int m_halfVisionCone = 80;
    public int m_visionRadius = 8;
    public bool m_drawVisionGizmo;
    protected LayerMask m_groundLayer = 1 << 9;
    protected LayerMask m_playerLayer = 1 << 10;
    public LayerMask m_obstacleLayer;
    protected Transform m_target;
    protected Vector3 m_curVel;//currentVelocity
    public float m_maxForce = 2;
    protected Vector3 m_startPos;
    protected Vector3 m_wanderTarget;

    public int GetCurrentHealth()
    { return m_currentHealth; }

    public abstract void OnDeath();
    public abstract void OnEnemyHit(int _damage, Transform _attacker);
    protected abstract void Idle();
    protected abstract void Attack();
    protected abstract void Wander();

    protected bool Sight()
    {
       // Debug.Log("yepp");
        Collider[] col = Physics.OverlapSphere(transform.position, m_visionRadius, m_playerLayer);
        if (col.Length == 0)
        {
            m_target = null;
            return false;
        }

        Vector2 tar = new Vector2(col[0].transform.position.x, col[0].transform.position.z) 
            - new Vector2(transform.position.x, transform.position.z);

        Vector2 me = new Vector2(transform.forward.x, transform.forward.z);
        m_Angle = Vector2.Angle(me, tar);
        if (m_Angle < m_halfVisionCone) 
        {
            m_target = col[0].transform;
            return true;
        }
        else
        {
            m_target = null;
            return false;
        }
    }

    #region DebugRenders
    void OnGUI()
    {
        if (m_drawVisionGizmo)
        {
            GUI.Label(new Rect(25, 25, 200, 40), "Angle Between Objects: " + m_Angle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        if(m_drawVisionGizmo)
        {
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * m_visionRadius));

            Vector3 left = Quaternion.Euler(0, m_halfVisionCone, 0) * transform.forward;
            Gizmos.DrawLine(transform.position, transform.position + (left.normalized * m_visionRadius));

            Vector3 right = Quaternion.Euler(0, -m_halfVisionCone, 0) * transform.forward;
            Gizmos.DrawLine(transform.position, transform.position + (right.normalized * m_visionRadius));
        }
    }
    #endregion

    #region Steering
    protected Vector3 PathSeek(Vector3 _target)
    {
        float speed = m_moveSpeed * Time.deltaTime;
        float force = m_maxForce * Time.deltaTime;

        Vector3 desVel = Vector3.Normalize(_target - transform.position) * speed;
        Vector3 steering = desVel - m_curVel;
        return steering;
    }

    protected Vector3 PathWander(float _lastWander, Vector3 _lastWanderTarget)
    {
        if (_lastWander == -1 || IsTimerDone(_lastWander, 2.5f))
        {
            Vector2 spot = (Random.insideUnitCircle * 10) + new Vector2(m_startPos.x, m_startPos.y);
            //Debug.Log(spot);
            _lastWander = Time.time;
            return new Vector3(spot.x, 0, spot.y);
        }
        else
        {
            //if (new Vector2(_lastWanderTarget.x, _lastWanderTarget.z) == new Vector2(transform.position.x, transform.position.z))
            if(Distance2D(_lastWanderTarget,transform.position) < 0.4f)
            {
                return PathWander(-1, Vector3.zero);
            }
            return _lastWanderTarget;
        }

    }

    protected Vector3 PathWanderBasic(Vector3 _lastWanderTarget)
    {
        if (Distance2D(_lastWanderTarget, transform.position) < 0.2f)
        {
            Vector2 spot = Random.insideUnitCircle * 10;
            spot += new Vector2(m_startPos.x, m_startPos.z);
            //Debug.Log(spot);
            int savecounter = 0;
            while (true)
            {
                if(Distance2D(spot,transform.position) >= 4.5f)
                {
                    break;
                }
                else
                {
                    spot = Random.insideUnitCircle * 10;
                    spot += new Vector2(m_startPos.x, m_startPos.z);
                }
                savecounter++;
                if(savecounter > 9999)
                {
                    Debug.Log("DEBUG: while loop went over 9999");
                    break;
                }
            }

            return new Vector3(spot.x, 0, spot.y);

        }
        else
        {
            return _lastWanderTarget;
        }
    }

    protected void PathSteering(Vector3 _v3)
    {
        _v3 += PathCollisionAvoid();

        float speed = m_moveSpeed * Time.deltaTime;
        float force = m_maxForce * Time.deltaTime;

        Vector3 steering = Vector3.zero;
        steering += _v3;

        steering = steering.normalized * force;

        m_curVel = Vector3.Normalize(m_curVel + steering) * speed;
        transform.position += m_curVel;
    }

    Vector3 PathCollisionAvoid()
    {
        Vector3 ahead = m_curVel.normalized * m_visionRadius + transform.position;

        Vector3 avoidence = Vector3.zero;
        Transform threat = PathMostThreat(ahead);
        if(threat != null)
        {
            avoidence.x = ahead.x - threat.position.x;
            avoidence.z = ahead.z - threat.position.z;

            avoidence = avoidence.normalized * (m_maxMoveSpeed);
            //Debug.Log("go around");
        }
        else
        {
            //no avoidence force
        }
            return avoidence;
    }
    
    Transform PathMostThreat(Vector3 _ahead)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Linecast(transform.position, _ahead, out hit, m_obstacleLayer))
        {
            return hit.transform;
        }
        else
        {
            return null;
        }

        
    }
    #endregion

    protected void LookAt(Transform _t)
    {
        transform.LookAt(_t);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    protected void LookAt(Vector3 _v3)
    {
        transform.LookAt(_v3);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    protected float Distance2D(Vector3 _a, Vector3 _b)
    {
        Vector2 a = new Vector2(_a.x, _a.z);
        Vector2 b = new Vector2(_b.x, _b.z);

        return Vector2.Distance(a, b);
    }

    protected float Distance2D(Vector2 _v2, Vector3 _v3)
    {
        Vector2 b = new Vector2(_v3.x, _v3.z);
        return Vector2.Distance(_v2, b);
    }

    protected bool IsTimerDone(float _since, float _howlong)
    {
        if(Time.time - _since >= _howlong)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}