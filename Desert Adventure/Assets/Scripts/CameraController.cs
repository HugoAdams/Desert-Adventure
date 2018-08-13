using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    NORMAL,
    ON_SHIP
}

public class CameraController : MonoBehaviour {

    public bool b_auto = true; // Auto rotate towards player facing dir
    public Transform m_target;

    float m_radius = 5; // How far away camera is
    float m_angle = 0; // Y-axis rotation
    float m_YDiff = 10; // X-axis rotation

    public float m_topAngle = 7;
    float m_baseTopAngle;

    Vector3 m_targetLastPos;
    public float m_yAxisSpeed = 100;
    public float m_xAxisSpeed = 10;

    public float m_timeTillCameraReset = 3;
    float m_cameraResetTimer;

    [Header("Camera mode settings")]
    public float m_boatStartY = 13;
    public float m_playerStartY = 8;
    public float m_boatCameraRadius = 14;
    public float m_playerCameraRadius = 7;

    void Start ()
    {
        m_targetLastPos = transform.position;
        m_baseTopAngle = 5;

        // Assuming starting with player
        SetToPlayerMode();
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

        Focus();
    }

    void ResetCameraLogic()
    {
        if (b_auto == false)
            return;

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
            m_angle -= m_yAxisSpeed * Time.deltaTime;
        }
        else
        {
            m_angle += m_yAxisSpeed * Time.deltaTime;
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

        transform.position = Vector3.Lerp(transform.position, new Vector3(x, m_target.position.y + m_YDiff, yz), 4 * Time.deltaTime);
        transform.LookAt(m_target);
    }

    void VertMove(bool _up)
    {//the axis amount will be multiplied by m_MaxMoveSpeed
        if(_up)
        {
            m_YDiff += m_xAxisSpeed * Time.deltaTime;
            if(m_YDiff > 15)
            {
                m_YDiff = 15;
            }
        }
        else
        {
            m_YDiff -= m_xAxisSpeed * Time.deltaTime;
            if(m_YDiff < 0.5f)
            {
                m_YDiff = 0.5f;
            }
        }
        Focus();
    }

    public void SetToBoatMode()
    {
        // Start behind player
        if (m_target)
        {
            Vector3 backDir = -m_target.forward;
            backDir.y = 0;
            m_angle = Mathf.Atan2(backDir.z, backDir.x) * Mathf.Rad2Deg;
        }

        m_radius = m_boatCameraRadius;
        m_YDiff = m_boatStartY;
    }

    public void SetToPlayerMode()
    {
        // Start behind player
        if (m_target)
        {
            Vector3 backDir = -m_target.forward;
            backDir.y = 0;
            m_angle = Mathf.Atan2(backDir.z, backDir.x) * Mathf.Rad2Deg;
        }

        m_radius = m_playerCameraRadius;
        m_YDiff = m_playerStartY;
    }
}
