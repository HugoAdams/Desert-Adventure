using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour {

    [Header("Control Flags")]
    public bool m_canControl;
    public bool m_canGoUpHills;

    [Header("Speed variables")]
    public float m_shittyMaxVelocity = 5;
    public float m_maxVelocity = 10;
    public float m_gravityAccel = 15;
    public float m_smoothTime = 0.5f;

    [Header("Rotation variables")]
    public float m_rotationSmoothTime = 0.2f;

    [Header("Control variables")]
    public float m_groundedRayLength = 0.2f;
    [Range(5, 90)] public float m_minAngleBeforeSliding = 20;
    [Range(5, 90)] public float m_shittyMaxSlopeAngle = 30;
    [Range(5, 90)] public float m_maxSlopeAngle = 60;

    [Header("Boat Pieces")]
    public Transform m_boatBase;
    public Transform m_boatMast;
    public Transform m_boatSail;
    public Transform m_tiller;

    float m_refRot;
    Vector3 m_currentVel, m_groundNormal;
    LayerMask m_terrainMask;
    Rigidbody m_rbody;

    float m_airGracePeriod;
    float m_inputStrength, m_refInputStrength;
    float m_xInput, m_zInput;
    bool m_grounded;

    private void Awake()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_terrainMask = 1 << LayerMask.NameToLayer("Terrain");
        m_grounded = false;
    }

    private void Update()
    {
        UpdateIfGrounded();
        UpdatePlayerInput();
        PlayerRotationLogic();
        PlayerMove();
        //StablizingLogic();
        SlidingSlopeLogic();

        // Apply gravity
        if (!m_grounded)
            m_currentVel += Vector3.down * m_gravityAccel * Time.deltaTime;
        else
            m_currentVel.y = 0;

        if (m_airGracePeriod > 0)
            m_airGracePeriod -= Time.deltaTime;

        m_rbody.velocity = m_currentVel;
    }

    void StablizingLogic()
    {
        // Try rotate towards stable x-rotation
        if (!m_grounded)
        {
            Vector3 xStable = transform.eulerAngles;
            xStable.x = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(xStable), 90 * Time.deltaTime);
        }

        // Always try to stablize z-rotation
        Vector3 zStable = transform.eulerAngles;
        zStable.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(zStable), 240 * Time.deltaTime);
    }

    void UpdatePlayerInput()
    {
        // User input 
        m_xInput = Input.GetAxisRaw("Horizontal");
        m_zInput = -Input.GetAxisRaw("Vertical");
    }

    void PlayerRotationLogic()
    {
        if (!m_grounded && m_airGracePeriod <= 0) // No rotation when not grounded
            return;

        if (!m_canControl && !m_canGoUpHills) // No rotation control when can't move
            return;


        // Base target y-rotation on player input
        if (Mathf.Abs(m_xInput) < 0.05f && Mathf.Abs(m_zInput) < 0.05f) // Insufficent user input to rotate
            return;

        // Rotate input by camera dir
        Vector3 turnDir = Camera.main.transform.rotation * new Vector3(m_xInput, 0, -m_zInput);
        turnDir.y = 0;

        float angle = Mathf.Atan2(turnDir.x, turnDir.z) * Mathf.Rad2Deg;

        // Now rotate towards target angle
        Vector3 newEulers = transform.eulerAngles;
        newEulers.y = angle;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(newEulers), 90 * Time.deltaTime);
    }

    void PlayerMove()
    {
        if (!m_grounded && m_airGracePeriod <= 0) // No grounded, no controlleded
        {
            m_currentVel = m_rbody.velocity;
            return;
        }

        float maxSlope = m_canGoUpHills ? m_maxSlopeAngle : m_shittyMaxSlopeAngle;
        float floorAngle = Mathf.Acos(Vector3.Dot(m_groundNormal, Vector3.up)) * Mathf.Rad2Deg;
        if (floorAngle > maxSlope)
        {
            m_inputStrength = 0;
            m_refInputStrength = 0;
            return; // Don't allow movement, return input back to zero
        }

        float maxVel = m_canGoUpHills ? m_maxVelocity : m_shittyMaxVelocity;
        float targetInputStr = Mathf.Max(Mathf.Abs(m_xInput), Mathf.Abs(m_zInput));

        m_inputStrength = Mathf.SmoothDamp(m_inputStrength, targetInputStr, ref m_refInputStrength, m_smoothTime);

        if (targetInputStr < 0.1f) // Don't do anything if no input
        {
            // Slow down 
            m_currentVel *= (1 - Time.deltaTime);
        }
        else
        {
            if (m_canControl || m_canGoUpHills) // Player has control!
                m_currentVel = transform.forward * maxVel * m_inputStrength;
        }
    }

    void UpdateIfGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(m_boatBase.position, -transform.up, out hit, m_groundedRayLength, m_terrainMask))
        {
            m_grounded = true;
            m_airGracePeriod = 0.3f;
            m_groundNormal = hit.normal;

            // Get average of 3 raycasts
            if (Physics.Raycast(m_boatBase.position + transform.forward * 0.4f, -transform.up, out hit, m_groundedRayLength * 4, m_terrainMask))
                m_groundNormal += hit.normal;

            if (Physics.Raycast(m_boatBase.position - transform.forward * 0.4f, -transform.up, out hit, m_groundedRayLength * 4, m_terrainMask))
                m_groundNormal += hit.normal;

            m_groundNormal = m_groundNormal.normalized;
        }
        else
            m_grounded = false;

    }

    void SlidingSlopeLogic()
    {
        if (!m_grounded) // Only do this if grounded
            return;

        float floorAngle = Mathf.Acos(Mathf.Abs(Vector3.Dot(m_groundNormal, Vector3.up))) * Mathf.Rad2Deg;
        if (floorAngle > m_minAngleBeforeSliding)
        {
            ApplySlideVelocity(floorAngle);
        }
    }

    void ApplySlideVelocity(float _floorAngle)
    {
        // Base added velocity dependent on current angle
        float percent = 0;
        float lowerLimit = 0.0f;
        float maxVel = m_canGoUpHills ? m_maxVelocity : m_shittyMaxVelocity;
        if (m_canGoUpHills)
            percent = Mathf.Clamp((_floorAngle - m_minAngleBeforeSliding) / (m_maxSlopeAngle - m_minAngleBeforeSliding), lowerLimit, 1);
        else
            percent = Mathf.Clamp((_floorAngle - m_minAngleBeforeSliding) / (m_shittyMaxSlopeAngle - m_minAngleBeforeSliding), lowerLimit, 1);

        if (_floorAngle < 70)
        {
            // Slide down acceleration
            Vector3 slideVel = Vector3.zero;
            slideVel.x += (1f - m_groundNormal.y) * m_groundNormal.x * (1f - 0.0f);
            slideVel.z += (1f - m_groundNormal.y) * m_groundNormal.z * (1f - 0.0f);
            m_currentVel += slideVel * m_maxVelocity * percent; // Base movement off max velocity. If slope too high, cancel out movement by max velocity
        }
        else
        {
            m_currentVel += Vector3.Cross(Vector3.up, m_groundNormal) * m_maxVelocity * percent;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_boatBase.position, m_boatBase.position - transform.up * m_groundedRayLength);
        Vector3 newPos = m_boatBase.position + transform.forward * 0.4f;
        Gizmos.DrawLine(newPos, newPos - transform.up * m_groundedRayLength);
        newPos = m_boatBase.position - transform.forward * 0.4f;
        Gizmos.DrawLine(newPos, newPos - transform.up * m_groundedRayLength);
    }
}
