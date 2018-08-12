using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterLogic : MonoBehaviour {

    public float m_rotationSpeed = 90;
    public float m_bobDistance = 0.5f;
    public float m_bobFrequency = 1;
    Quaternion m_randomRotation;
    Vector3 m_startPos;

    private void Awake()
    {
        m_randomRotation = Random.rotationUniform;
        m_startPos = transform.position;
    }

    private void Update()
    {
        float rot = m_rotationSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(rot, rot, rot);

        transform.position = m_startPos + Vector3.up * Mathf.Sin(m_bobFrequency * Time.time) * m_bobDistance;
    }
}
