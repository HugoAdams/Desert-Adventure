using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected int m_currentHealth;
    [Range(1, 40)] public int m_MaxHealth = 10;
    [Range(1, 100)] public float m_moveSpeed = 20.0f;

    public int GetCurrentHealth()
    { return m_currentHealth; }

    public abstract void OnDeath();
    public abstract void OnEnemyHit(int _damage);
    public abstract void Idle();
    public abstract void Attack();


}
