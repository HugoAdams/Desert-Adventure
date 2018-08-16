using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsCanvas : MonoBehaviour {

    public Transform m_controls;

    private void Start()
    {
        m_controls.gameObject.SetActive(false);
    }

    void Update ()
    {
        if (Input.GetButtonDown("ShowControls"))
        {
            m_controls.gameObject.SetActive(!m_controls.gameObject.activeSelf);
        }
	}
}
