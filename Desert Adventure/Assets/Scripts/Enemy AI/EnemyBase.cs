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
        NULL
    }

    float  m_Angle = 0;
    protected int m_currentHealth;
    [Range(1, 40)] public int m_MaxHealth = 10;
    [Range(1, 100)] public float m_moveSpeed = 20.0f;

    [Header("Vision Stuff")]
    [Range(10, 180)] public int m_halfVisionCone = 80;
    public int m_visionRadius = 8;
    public bool m_drawVisionGizmo;
    protected LayerMask m_groundLayer = 1 << 9;
    protected LayerMask m_playerLayer = 1 << 10;
    protected Transform m_target;

    public int GetCurrentHealth()
    { return m_currentHealth; }

    public abstract void OnDeath();
    public abstract void OnEnemyHit(int _damage, Transform _attacker);
    public abstract void Idle();
    public abstract void Attack();
    public abstract void Wander();

    protected bool Sight()
    {
       // Debug.Log("yepp");
        Collider[] col = Physics.OverlapSphere(transform.position, m_visionRadius, m_playerLayer);
        if (col.Length == 0)
        {
            return false;
        }

        Vector2 tar = new Vector2(col[0].transform.position.x, col[0].transform.position.z) 
            - new Vector2(transform.position.x, transform.position.z);

        Vector2 me = new Vector2(transform.forward.x, transform.forward.z);
        m_Angle = Vector2.Angle(me, tar);
        if (Vector2.Angle(me, tar) < m_halfVisionCone) 
        {
            m_target = col[0].transform;
            return true;
        }
        else
        {
            //Debug.Log("out");
            return false;
        }
    }

    #region DebugRenders
    void OnGUI()
    {
        //Output the angle found above
        //GUI.Label(new Rect(25, 25, 200, 40), "Angle Between Objects: " + m_Angle);
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
}
#endregion