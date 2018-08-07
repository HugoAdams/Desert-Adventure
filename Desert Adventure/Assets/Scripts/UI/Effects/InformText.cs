using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformText : MonoBehaviour {

    public float m_fadeInTime = 0.2f;
    public float m_idleTime = 1;
    public float m_fadeOutTime = 1;
    public float m_floatSpeed = 30;

    TextMeshProUGUI m_tmp;

    private void Awake()
    {
        m_tmp = GetComponent<TextMeshProUGUI>();
        m_tmp.color = new Color(m_tmp.color.r, m_tmp.color.g, m_tmp.color.b, 0);
    }

    public void InitializeAndStart(string _text)
    {
        m_tmp.text = _text;
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        // Fade in
        while (m_tmp.color.a < 1)
        {
            m_tmp.color = new Color(m_tmp.color.r, m_tmp.color.g, m_tmp.color.b, m_tmp.color.a + Time.deltaTime / m_fadeInTime);
            yield return null;
        }

        // Idle time
        float startTime = Time.time;
        while (Time.time - startTime < m_idleTime)
        {
            transform.position += Vector3.up * m_floatSpeed * Time.deltaTime;
            yield return null;
        }

        // Fade out
        while (m_tmp.color.a > 0)
        {
            m_tmp.color = new Color(m_tmp.color.r, m_tmp.color.g, m_tmp.color.b, m_tmp.color.a - Time.deltaTime / m_fadeOutTime);
            transform.position += Vector3.up * m_floatSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }
}
