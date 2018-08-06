using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour {

    [Header("Speed variables")]
    public float m_forceApplied = 5000;
    public float m_maxVelocity = 10;
    Vector3 m_currentVel, m_refVel;

    Rigidbody m_rbody;  

    private void Awake()
    {
        m_rbody = GetComponent<Rigidbody>();
    }

    
}
