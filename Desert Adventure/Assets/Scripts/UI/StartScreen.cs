using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class StartScreen : MonoBehaviour {

    public Image m_fadeScreen;
    public Button m_defaultButton;
    public TextMeshProUGUI m_fadingText;

    public RectTransform[] m_spinningPieces;
    public RectTransform[] m_leftGroup;
    public RectTransform[] m_rightGroup;


    bool m_menuStarted;

    private void Start()
    {
        m_fadeScreen.color = new Color(m_fadeScreen.color.r, m_fadeScreen.color.g, m_fadeScreen.color.b, 1);
        m_menuStarted = false;
        StartCoroutine(FadeOut());
    }

    private void Update()
    {
        if (!m_menuStarted)
        {
            if (Input.GetButtonDown("Jump"))
            {
                m_menuStarted = true;
                StartCoroutine(StartMenuAnimation());
            }

            for (int i = 0; i < m_spinningPieces.Length; i++)
            {
                float rotateDir = i % 2 == 0 ? -1 : 1;
                Vector3 rotate = new Vector3(0, 0, rotateDir * 45);

                m_spinningPieces[i].Rotate(rotate * Time.deltaTime);
            }
        }
    }

    IEnumerator FadeOut()
    {
        while (m_fadeScreen.color.a > 0)
        {
            m_fadeScreen.color = new Color(m_fadeScreen.color.r, m_fadeScreen.color.g, m_fadeScreen.color.b, m_fadeScreen.color.a - Time.deltaTime * 0.3f);
            yield return null;
        }
    }

    IEnumerator StartMenuAnimation()
    {
        Destroy(m_fadingText.gameObject);

        // Rotate all pieces to correct position
        float rotateSpeed = 90;
        float openGateSpeed = (Screen.width / 1920.0f) * 160;
        float openGateTime = 2f;

        bool doneRotating = false;
        while (!doneRotating)
        {
            doneRotating = true;

            // Rotate each piece, also check if they are finished
            foreach (RectTransform rt in m_spinningPieces)
            {
                rt.rotation = Quaternion.RotateTowards(rt.rotation, Quaternion.identity, rotateSpeed * Time.deltaTime);
                if (rt.rotation != Quaternion.identity) // There is a puzzle piece not done rotating
                    doneRotating = false;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.7f);

        // Open gate animation
        float startTime = Time.time;

        while (Time.time - startTime < openGateTime)
        {
            foreach (RectTransform rt in m_leftGroup)
            {
                rt.position -= Vector3.right * Time.deltaTime * openGateSpeed;
            }

            foreach (RectTransform rt in m_rightGroup)
            {
                rt.position += Vector3.right * Time.deltaTime * openGateSpeed;
            }

            yield return null;
        }

        // Menu start
        EventSystem.current.SetSelectedGameObject(m_defaultButton.gameObject);

        yield return null;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitProgram()
    {
        Application.Quit();
    }
}
