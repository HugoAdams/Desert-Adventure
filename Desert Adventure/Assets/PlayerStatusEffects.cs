using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour {

    public float m_flattenedRecoveryTime;
    public float m_flattenedStunTime;
    private float m_flattenedTimeStamp;

    private Transform m_model;
    private bool m_isFlattened;

    public delegate void OnStunned();
    public delegate void OnUnStunned();
    public event OnStunned m_onStunned;
    public event OnUnStunned m_onUnStunned;

    private void Awake()
    {
        m_model = transform.Find("Model");
    }

    public void Flattened()
    {
        if (!m_isFlattened)
        {
            m_onStunned();
            m_isFlattened = true;
            m_model.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
            m_model.localScale = new Vector3(1.0f, 0.01f, 1.0f);
            StartCoroutine(FlattenedRecovery());
            StartCoroutine(StunnedRecovery(m_flattenedStunTime));
        };
    }

    IEnumerator StunnedRecovery(float stunnedTime)
    {
        yield return new WaitForSeconds(stunnedTime);
        m_onUnStunned();
        yield return null;
    }

        IEnumerator FlattenedRecovery()
    {
        m_flattenedTimeStamp = Time.time;
        while ((m_flattenedTimeStamp + m_flattenedRecoveryTime) >= Time.time)
        {
            float scas = ((Mathf.Sin(Time.time * 8) + 1.0f) * 0.2f) + 0.8f;
            m_model.localScale = new Vector3(scas, 0.01f, scas);
            yield return null;
        }

        m_model.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        m_model.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        m_isFlattened = false;
        yield return null;
    }
}
