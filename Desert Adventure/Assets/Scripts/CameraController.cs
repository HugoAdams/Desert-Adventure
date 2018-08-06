using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    float m_angle = 0;
    public float m_topAngle = 0;
    public Transform m_target;
    public float m_radius = 5;
    Vector3 m_targetLastPos;
    public float m_MaxTurnSpeed = 100;

    public float m_MaxMoveSpeed = 10;
    float m_YDiff = 10;

    // Use this for initialization
    void Start ()
    {
        m_targetLastPos = transform.position;
	}
	
	// Update is called once per frame

    void Update()
    {
       
        if(Input.GetKey(KeyCode.Z))
        {
            Rotate(true);
        }
        if(Input.GetKey(KeyCode.X))
        {
            Rotate(false);
        }
        if(Input.GetKey(KeyCode.F))
        {
            VertMove(true);
        }
        if (Input.GetKey(KeyCode.V))
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
            if(m_YDiff < -1)
            {
                m_YDiff = -1;
            }
        }
        Focus();
    }
}
