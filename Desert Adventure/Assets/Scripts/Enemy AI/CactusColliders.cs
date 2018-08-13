using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusColliders : MonoBehaviour {

    public CapsuleCollider m_bodyTrigger = null;
    public CapsuleCollider m_spearTrigger = null;
    public CapsuleCollider m_bodyCollder = null;

    public void DisableAll()
    {
        if(m_bodyTrigger != null)
        {
            m_bodyTrigger.enabled = false;
        }

        if(m_spearTrigger != null)
        {
            m_spearTrigger.enabled = false;
        }

        if (m_bodyCollder != null)
        {
            m_bodyCollder.enabled = false;
        }
    }
}
