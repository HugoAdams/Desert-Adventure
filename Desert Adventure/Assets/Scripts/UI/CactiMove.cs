using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactiMove : MonoBehaviour {

    public Vector3 m_offset;
    Vector3 m_trueOffset, m_startPos, m_targetPos;
    RectTransform m_rt;

    private void Awake()
    {
        m_rt = GetComponent<RectTransform>();
        m_startPos = m_rt.anchoredPosition;
        m_trueOffset = (Screen.width / 1920.0f) * m_offset;
        m_targetPos = m_startPos;
    }

    private void Update()
    {
        m_rt.anchoredPosition = Vector3.Lerp(m_rt.anchoredPosition, m_targetPos, 15 * Time.deltaTime);        
    }

    public void ChangeTarget(bool _toTargetPos)
    {
        m_targetPos = _toTargetPos ? m_startPos + m_trueOffset : m_startPos;
    }

}
