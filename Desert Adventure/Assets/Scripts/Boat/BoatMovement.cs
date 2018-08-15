﻿using System.Collections;
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
    public float m_driveForce = 1500.0f;
    public float m_shittyDriveForce = 1000.0f;
    public float m_airTimeTorque = 300.0f;

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
    public Transform m_boatTiller;
    public Transform m_playerStandPoint;

    float m_refRot;
    Vector3 m_currentVel, m_groundNormal, m_currentSlideVel;
    LayerMask m_terrainMask;
    Rigidbody m_rbody;

    float m_rayCastDistApart = 1.2f;
    float m_airGracePeriod;
    float m_inputStrength, m_refInputStrength;
    float m_xInput, m_zInput;
    public bool m_grounded;

    PlayerController m_player;

    private void Awake()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_terrainMask = 1 << LayerMask.NameToLayer("Terrain");
        m_grounded = false;
        m_player = null;
    }

    private void Update()
    {
        CheckGroundLogic();
        UpdatePlayerInput();
        PlayerRotationLogic();
        //PlayerMove();
        //StablizingLogic();
        //SlidingSlopeLogic();
        DismountLogic();

        if (m_airGracePeriod > 0)
            m_airGracePeriod -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        if (m_canControl)
        {
            ForceLogic();
            //AirControlLogic();
        }
    }

    void AirControlLogic()
    {
        if (m_grounded || m_airGracePeriod > 0)
            return;

        // Rotate around depending on input
        Vector3 torque = new Vector3(m_zInput, 0, -m_xInput).normalized * m_airTimeTorque * Time.fixedDeltaTime;
        m_rbody.AddRelativeTorque(torque, ForceMode.Acceleration);

        if (m_rbody.angularVelocity.sqrMagnitude > 9)
        {
            m_rbody.angularVelocity = m_rbody.angularVelocity.normalized * 3;
        }
    }

    void ForceLogic()
    {
        if (!m_grounded && m_airGracePeriod <= 0)
            return;

        float maxSlope, driveForce, maxVel;

        if (m_canGoUpHills)
        {
            maxSlope = m_maxSlopeAngle;
            driveForce = m_driveForce;
            maxVel = m_maxVelocity;
        }
        else
        {
            maxSlope = m_shittyMaxSlopeAngle;
            driveForce = m_shittyDriveForce;
            maxVel = m_shittyMaxVelocity;
        }

        float floorAngle = Mathf.Acos(Mathf.Abs(Vector3.Dot(m_groundNormal, Vector3.up))) * Mathf.Rad2Deg;
        if (floorAngle > maxSlope)
            return;

        if (Mathf.Abs(m_xInput) > 0.1f || Mathf.Abs(m_zInput) > 0.1f)
        {
            Vector3 dir = transform.forward + Vector3.up * 0.05f;
            m_rbody.AddForce(dir * driveForce * Time.deltaTime, ForceMode.Acceleration);

            // Limit velocity Logic
            Vector3 move = m_rbody.velocity;
            move.y = 0;
            if (move.sqrMagnitude > maxVel * maxVel)
            {
                float yMove = m_rbody.velocity.y;
                m_rbody.velocity = move.normalized * maxVel + Vector3.up * yMove;
            }
        }
    }

    void DismountLogic()
    {
        if (!m_player) // No player to dismount to
            return;

        if (Input.GetButtonDown("BoatMounting"))
        {
            // Dismount!
            m_player.transform.SetParent(null);
            m_player.DismountBoat(m_rbody.velocity * 0.5f + Vector3.up * 14);
            Destroy(gameObject);
        }
    }

    void StablizingLogic()
    {
        // Try rotate towards stable x-rotation
        if (!m_grounded)
        {
            if (Mathf.Abs(transform.eulerAngles.x) > 30.0f)
            {
                Vector3 torque = new Vector3(-transform.eulerAngles.x, 0 ,0).normalized * m_airTimeTorque;
                m_rbody.AddTorque(torque);
                //Debug.Log("TRYING!");
            }
        }

        // Always try to stablize z-rotation
        if (Mathf.Abs(transform.eulerAngles.z) > 30.0f)
        {
            Vector3 torque = new Vector3(transform.eulerAngles.z, 0, 0).normalized * m_airTimeTorque;
            m_rbody.AddTorque(torque);
        }

        if (m_rbody.angularVelocity.sqrMagnitude > 9)
        {
            m_rbody.angularVelocity = m_rbody.angularVelocity.normalized * 3;
        }
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


        float rotateSpeed = m_canGoUpHills ? 145 : 90;

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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(newEulers), rotateSpeed * Time.deltaTime);
    }

    void PlayerMove()
    {
        // Input strength
        float maxVel = m_canGoUpHills ? m_maxVelocity : m_shittyMaxVelocity;
        float targetInputStr = Mathf.Max(Mathf.Abs(m_xInput), Mathf.Abs(m_zInput));

        if (!m_grounded && m_airGracePeriod <= 0) // No grounded, no controlleded
        {
            // Input strength will decrease over time when not grounded
            m_inputStrength = Mathf.SmoothDamp(m_inputStrength, targetInputStr, ref m_refInputStrength, m_smoothTime * 0.5f);
            m_currentVel = m_rbody.velocity;
            return;
        }

        if (targetInputStr > 0.05f)
            m_inputStrength = Mathf.SmoothDamp(m_inputStrength, targetInputStr, ref m_refInputStrength, m_smoothTime);
        else
            m_inputStrength = Mathf.SmoothDamp(m_inputStrength, targetInputStr, ref m_refInputStrength, m_smoothTime * 0.5f);

        float maxSlope = m_canGoUpHills ? m_maxSlopeAngle : m_shittyMaxSlopeAngle;
        float floorAngle = Mathf.Acos(Vector3.Dot(m_groundNormal, Vector3.up)) * Mathf.Rad2Deg;
        if (floorAngle > maxSlope)
        {
            m_inputStrength = 0;
            m_refInputStrength = 0;
            return; // Don't allow movement, return input back to zero
        }

        if (targetInputStr < 0.1f) // Don't do anything if no input
        {
            // Slow down 
            m_currentVel *= (1 - Time.deltaTime);
        }
        else
        {
            if (m_canControl || m_canGoUpHills) // Player has control!
            {
                // Test #12,332
                // Use weighted-average of ground normal, & facing direction
                float weight = 1.3f;
                Vector3 dir = (Vector3.Cross(transform.right, m_groundNormal) * weight + transform.forward * (2.0f - weight)) * 0.5f;
                m_currentVel =  dir * maxVel * m_inputStrength;
            }
        }
    }

    void CheckGroundLogic()
    {
        RaycastHit hit;
        float count = 1;

        if (m_grounded)
            m_airGracePeriod = 0.3f;

        if (Physics.Raycast(m_boatBase.position + m_boatBase.up * 0.08f - transform.forward * m_rayCastDistApart, -transform.up, out hit, m_groundedRayLength, m_terrainMask))
        {
            m_groundNormal = hit.normal;

            // Normal = from of boat
            if (Physics.Raycast(m_boatBase.position + m_boatBase.up * 0.08f +  transform.forward * m_rayCastDistApart, -transform.up, out hit, m_groundedRayLength * 4, m_terrainMask))
            {
                m_groundNormal += hit.normal*2;
                count += 2;
            }

            if (Physics.Raycast(m_boatBase.position + m_boatBase.up * 0.08f, -transform.up, out hit, m_groundedRayLength * 4, m_terrainMask))
            {
                m_groundNormal += hit.normal;
                count += 1;
            }

            m_groundNormal /= count;
        }
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
            slideVel *= m_maxVelocity * percent;
            m_currentSlideVel = Vector3.Lerp(m_currentSlideVel, slideVel, 3 * Time.deltaTime);
        }
        else
        {
            Vector3 superSlideVel = Vector3.Cross(Vector3.up, m_groundNormal) * m_maxVelocity * percent;
            m_currentSlideVel = Vector3.Lerp(m_currentSlideVel, superSlideVel, Time.deltaTime);
        }
        m_currentVel += m_currentSlideVel;

    }

    public void Initialize(PlayerStats _pStats, PlayerController _player)
    {
        m_player = _player;

        // Set the player onto boat position
        m_player.transform.SetParent(transform);
        m_player.transform.position = m_playerStandPoint.position;
        m_player.transform.rotation = m_playerStandPoint.rotation;

        if (_pStats.BoatTiller && _pStats.BoatSail && _pStats.BoatMast)
        {
            // Full boat!
            m_canControl = m_canGoUpHills = true;
        }
        else if (_pStats.BoatTiller || (_pStats.BoatSail && _pStats.BoatMast))
        {
            // Shit control
            m_canControl = true;
            m_canGoUpHills = false;
        }
        else
        {
            // No Control at all!
            m_canGoUpHills = m_canControl = false;
        }

        UpdateBoatPieces(_pStats);
    }

    void UpdateBoatPieces(PlayerStats _pStats)
    {
        // Mast 
        if (!_pStats.BoatMast)
            m_boatMast.gameObject.SetActive(false);

        // Sail
        if (!_pStats.BoatMast || !_pStats.BoatSail)
            m_boatSail.gameObject.SetActive(false);

        // Tiller
        if (!_pStats.BoatTiller)
            m_boatTiller.gameObject.SetActive(false);

        // Base should always be there!
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
    }
}
