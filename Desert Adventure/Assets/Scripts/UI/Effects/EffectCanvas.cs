using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCanvas : MonoBehaviour {

    public static EffectCanvas Instance
    { get; private set;}

    public Transform m_informTextPrefab;
    public Transform m_titleTextPrefab;
    public Transform m_helpTextPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Kinda like error message for user
    public void InformText(string _text) { SummonTextAnimation(_text, m_informTextPrefab); }

    // Big title, for like new zone
    public void TitleText(string _text) { SummonTextAnimation(_text, m_titleTextPrefab); }

    // Like telling player controls or something
    public void HelperText(string _text) { SummonTextAnimation(_text, m_helpTextPrefab); }

    public void SummonTextAnimation(string _text, Transform _prefab)
    {
        Transform newText = Instantiate(_prefab, _prefab.position, _prefab.rotation, transform);
        newText.GetComponent<RectTransform>().anchoredPosition = _prefab.position;
        newText.GetComponent<InformText>().InitializeAndStart(_text);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            InformText("SAD <color=blue> SAD </color> <sprite=4>");

        if (Input.GetKeyDown(KeyCode.O))
            TitleText("OBJECTIVE: GET SOME SLEEP SOMETIME SOON <sprite=6>");

        if (Input.GetKeyDown(KeyCode.I))
            HelperText("PRESS <sprite=4> to get on boat");
    }

}
