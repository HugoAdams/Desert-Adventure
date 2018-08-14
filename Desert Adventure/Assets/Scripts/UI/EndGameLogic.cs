using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndGameLogic : MonoBehaviour {

    public TextMeshProUGUI m_titleUI;
    public TextMeshProUGUI m_quitUI;
    public Image m_background;

    bool m_canQuit = false;

    private void Awake()
    {
        m_titleUI.color = new Color(m_titleUI.color.r, m_titleUI.color.g, m_titleUI.color.b, 0);
        m_quitUI.color = new Color(m_quitUI.color.r, m_quitUI.color.g, m_quitUI.color.b, 0);
        m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, 0);
    }

    private void Update()
    {
        if (!m_canQuit)
            return;

        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Start()
    {
        StartCoroutine(EndAnimation());
    }

    IEnumerator EndAnimation()
    {
        float fadeInTime = 2.0f;

        yield return new WaitForSeconds(1.0f);

        // Fade in stuff
        while (m_titleUI.color.a < 1)
        {
            float increment = m_titleUI.color.a + Time.deltaTime / fadeInTime;
            m_titleUI.color = new Color(m_titleUI.color.r, m_titleUI.color.g, m_titleUI.color.b, increment);
            m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, increment);
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        m_canQuit = true;

        while (m_quitUI.color.a < 1)
        {
            float increment = m_quitUI.color.a + Time.deltaTime / fadeInTime;
            m_quitUI.color = new Color(m_quitUI.color.r, m_quitUI.color.g, m_quitUI.color.b, increment);
            yield return null;
        }

        yield return null;
    }

}
