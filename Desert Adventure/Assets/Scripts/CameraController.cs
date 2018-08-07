﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    NORMAL,
    ON_SHIP
}

public class CameraController : MonoBehaviour {

    public Transform m_target;

    public float m_radius = 5;
    float m_angle = 0;

    public float m_topAngle = 7;
    float m_baseTopAngle;

    Vector3 m_targetLastPos;
    public float m_MaxTurnSpeed = 100;

    public float m_MaxMoveSpeed = 10;
    float m_YDiff = 10;

    public float m_timeTillCameraReset = 3;
    float m_cameraResetTimer;

    void Start ()
    {
        m_targetLastPos = transform.position;
        m_baseTopAngle = 5;
    }
	
	// Update is called once per frame

    void Update()
    {
        float xInput = Input.GetAxisRaw("CameraHorizontal");
        float yInput = Input.GetAxisRaw("CameraVertical");

        if (Mathf.Abs(xInput) > 0.1f || Mathf.Abs(yInput) > 0.1f)
            m_cameraResetTimer = m_timeTillCameraReset;

        if (m_cameraResetTimer > 0)
            m_cameraResetTimer -= Time.deltaTime;

        if (m_cameraResetTimer <= 0)
            ResetCameraLogic();

        if (xInput > 0)
        {
            Rotate(true);
        }
        if(xInput < 0)
        {
            Rotate(false);
        }
        if(yInput > 0)
        {
            VertMove(true);
        }
        if (yInput < 0)
        {
            VertMove(false);
        }

        if (!m_target)
            return;
        if(m_target.position != m_targetLastPos)
        {
            Focus();
            m_targetLastPos = m_target.position;
        }
    }

    void ResetCameraLogic()
    {
        float moveAmount = 4;
        float rotateAmount = 20;

        // Move towards angles
        m_YDiff -= Mathf.Clamp(m_YDiff - m_baseTopAngle, -moveAmount * Time.deltaTime, moveAmount * Time.deltaTime);

        Vector3 backDir = -m_target.forward;
        backDir.y = 0;
        float angle = Mathf.Atan2(backDir.z, backDir.x) * Mathf.Rad2Deg;
        m_angle = Mathf.MoveTowardsAngle(m_angle, angle, rotateAmount * Time.deltaTime);
        Focus();
    }


    void Rotate(bool _left)
    {//the horizontal axis amount will multiply m_MaxTurnSpeed
        if (_left)
        {
            m_angle -= m_MaxTurnSpeed * Time.deltaTime;
        }
        else
        {
            m_angle += m_MaxTurnSpeed * Time.deltaTime;
        }
        if (m_angle > 360)
        {
            m_angle -= 360;
        }
        else if (m_angle < 0)
        {
            m_angle += 360;
        }

        Focus();
    }


    void Focus()
    {

        float x = m_radius * Mathf.Cos(m_angle * Mathf.Deg2Rad);
        float yz = m_radius * Mathf.Sin(m_angle * Mathf.Deg2Rad);

        x += m_target.position.x;
        yz += m_target.position.z;

        transform.position = new Vector3(x, m_target.position.y + m_YDiff, yz);


        // transform.LookAt(center);
        transform.LookAt(m_target);

        //transform.eulerAngles = new Vector3(45, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void VertMove(bool _up)
    {//the axis amount will be multiplied by m_MaxMoveSpeed
        if(_up)
        {
            m_YDiff += m_MaxMoveSpeed * Time.deltaTime;
            if(m_YDiff > 15)
            {
                m_YDiff = 15;
            }
        }
        else
        {
            m_YDiff -= m_MaxMoveSpeed * Time.deltaTime;
            if(m_YDiff < 3)
            {
                m_YDiff = 3;
            }
        }
        Focus();
    }
}
