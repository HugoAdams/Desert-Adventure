using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    public float m_buttonCollisionRadius;

    private Rigidbody m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
		if(m_rb.velocity != Vector3.zero)
            CheckIfActivatingButton();
	}

    // Checks if the rock is close enough to activate a rock button
    void CheckIfActivatingButton()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_buttonCollisionRadius, 1 << 12);

        if (hitColliders.Length == 0)
            return;

        GameObject closestButton = null;
        Vector3 currentPos = transform.position;
        float minDist = 10000;

        for (int i = 0; i < hitColliders.Length; ++i)
        {
            if(hitColliders[i].GetComponent<RockButton>().m_rockPlaced) // The button already has a rock on it
                continue;

            float dist = Vector3.Distance(hitColliders[i].transform.position, currentPos);
            if (dist < minDist) // The button is closer distance to the rock than the previously selected button
            {
                closestButton = hitColliders[i].gameObject;
                minDist = dist;
            }
        }

        if (closestButton == null)
            return;

        closestButton.GetComponent<RockButton>().TriggerRockPlaced(new Quaternion(0, transform.rotation.y, 0, 1));
        Destroy(gameObject);
    }
}
