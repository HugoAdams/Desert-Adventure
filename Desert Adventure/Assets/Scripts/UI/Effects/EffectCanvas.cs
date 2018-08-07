using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCanvas : MonoBehaviour {

    public static EffectCanvas Instance
    { get; private set;}

    public Transform m_informTextPrefab;
    public Transform m_titleTextPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void InformText(string _text)
    {
        Transform newText = Instantiate(m_informTextPrefab, m_informTextPrefab.position, m_informTextPrefab.rotation, transform);
        newText.GetComponent<RectTransform>().anchoredPosition = m_informTextPrefab.position;
        newText.GetComponent<InformText>().InitializeAndStart(_text);
    }

    public void TitleText(string _text)
    {
        Transform newText = Instantiate(m_titleTextPrefab, m_informTextPrefab.position, m_informTextPrefab.rotation, transform);
        newText.GetComponent<RectTransform>().anchoredPosition = m_titleTextPrefab.position;
        newText.GetComponent<InformText>().InitializeAndStart(_text);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            InformText("SAD <color=blue> SAD </color> <sprite=4>");

        if (Input.GetKeyDown(KeyCode.O))
            TitleText("OBJECTIVE: GET SOME SLEEP SOMETIME SOON <sprite=6>");
    }

}
