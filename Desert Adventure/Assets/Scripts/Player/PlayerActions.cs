using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    public KeyCode m_pickupKey;

    private bool m_pickupInRange;

	// Update is called once per frame
	void Update () {
	}

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Rock")
            m_pickupInRange = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Rock")
            m_pickupInRange = false;
    }
}
