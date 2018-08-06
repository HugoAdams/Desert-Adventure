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

    private void Update()
    {
        float currentVel = m_rbody.velocity.y;
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        Vector3 vel = Camera.main.transform.rotation * new Vector3(xInput, 0, zInput);
        vel.y = 0;
        vel = vel.normalized;
        vel *= m_maxVelocity;
        vel.y = currentVel;

        m_rbody.velocity = vel;
        
    }
}
