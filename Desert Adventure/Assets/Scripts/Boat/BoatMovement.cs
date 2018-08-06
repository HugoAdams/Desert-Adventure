using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour {

    [Header("Speed variables")]
    public float m_forceApplied = 5000;
    public float m_maxVelocity = 10;

    [Range(5, 90)]
    public float m_minAngleBeforeSliding = 20;

    Vector3 m_currentVel, m_refVel;
    LayerMask m_terrainMask;
    Rigidbody m_rbody;  

    private void Awake()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_terrainMask = 1 << LayerMask.NameToLayer("Terrain");
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
        SlidingSlopeLogic();
    }

    void SlidingSlopeLogic()
    {
        // Get normal of ground
        RaycastHit hit;

        // Only on grounded slope
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f, m_terrainMask))
        {
            Vector3 normal = hit.normal;
            float floorAngle = Mathf.Acos(Vector3.Dot(normal, Vector3.up)) * Mathf.Rad2Deg;
            if (floorAngle > m_minAngleBeforeSliding)
            {
                // Slide down acceleration
                Vector3 slideVel = Vector3.zero;
                slideVel.x += (1f - normal.y) * normal.x * (1f - 0.0f);
                slideVel.z += (1f - normal.y) * normal.z * (1f - 0.0f);
                m_rbody.velocity += slideVel;
            }
        }
    }
}
