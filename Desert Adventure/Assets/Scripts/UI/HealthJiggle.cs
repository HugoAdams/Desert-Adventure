using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthJiggle : MonoBehaviour {

    [MinMaxRange(-90, 90)]
    public RangedFloat m_targetRotation;

    [MinMaxRange(0.1f, 15.0f)]
    public RangedFloat m_restTime;

    bool m_rotating;
    float m_targetTime;
    float m_currentTargetRotation;

    private void Start()
    {
        SetNextRotation(true);
    }

    void SetNextRotation(bool _startOfGame = false)
    {
        m_rotating = false;
        if (!_startOfGame)
            m_targetTime = Random.Range(m_restTime.minValue, m_restTime.maxValue) + Time.time;
        else
            m_targetTime = Random.Range(m_restTime.minValue * 0.2f, m_restTime.maxValue * 0.2f) + Time.time;

        if (m_currentTargetRotation != 0)
            m_currentTargetRotation = 0;
        else
            m_currentTargetRotation = Random.Range(m_targetRotation.minValue, m_targetRotation.maxValue);
    }

    private void Update()
    {
        if (m_rotating)
        {
            Quaternion targetRot = Quaternion.AngleAxis(m_currentTargetRotation, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 90.0f * Time.deltaTime);

            if (transform.rotation == targetRot)
                SetNextRotation();
        }
        else if (Time.time >= m_targetTime)
            m_rotating = true;
    }
}
