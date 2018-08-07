using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    public Transform lookAt;
    public float distance = 10.0f;
    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;

    private Transform camTransform;

    private float currentX = 0.0f;
    private float currentY = 40.0f;

    private Vector3 m_defaultPosition;
    private Quaternion m_defaultRotation;

    private bool m_playerMovedCamera;
    public float m_resetPositionDelayTime;
    private float m_resetPositionTimeStamp;

    private void Start()
    {
        camTransform = transform;
        m_defaultPosition = transform.position;
        m_defaultRotation = transform.rotation;
    }

    private void Update()
    {
        // Move the camera with the players movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY += Input.GetAxis("Mouse Y") * sensitivityY;

            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
            m_playerMovedCamera = true;
            m_resetPositionTimeStamp = Time.time;
        }
        // Reset the camera to the default position
        else
        {
            m_playerMovedCamera = false;
        }
    }

    private void LateUpdate()
    {
        if (!m_playerMovedCamera && (m_resetPositionTimeStamp + m_resetPositionDelayTime) < Time.time)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, m_defaultRotation, 50 * Time.deltaTime);
            Vector3 dir = new Vector3(0, 0, -distance);
            camTransform.position = lookAt.position + transform.rotation * dir;
            camTransform.LookAt(lookAt.position);
            currentX += transform.rotation.x;
            currentY += transform.rotation.y;
        }
        else
        { 
            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            camTransform.position = lookAt.position + rotation * dir;
            camTransform.LookAt(lookAt.position);
        }
    }
}
