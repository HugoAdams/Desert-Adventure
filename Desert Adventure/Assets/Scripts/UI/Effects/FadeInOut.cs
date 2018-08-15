using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeInOut : MonoBehaviour {

    TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update ()
    {
        float alpha = 0.2f * Mathf.Sin(Time.time * 10) + 0.8f;
        tmp.color = new Color(1, 1, 1, alpha);
	}
}
