using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    public KeyCode m_pickupKey;
    public float m_pickUpRangeRadius;
    public float m_forwardThrowStrength;
    public float m_upwardsThrowStrength;

    private bool m_holdingObject;
    private GameObject m_pickup;

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(m_pickupKey))
        {
            if (!m_holdingObject)
            {
                m_pickup = GetClosestPickup();
                if (m_pickup != null)
                {
                    m_pickup.transform.rotation = new Quaternion(0, m_pickup.transform.rotation.y, 0, 1);
                    m_pickup.transform.SetParent(transform);
                    m_pickup.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                    DisableRagdoll(m_pickup.GetComponent<Rigidbody>());
                    m_holdingObject = true;
                }
            }
            else
            {
                EnableRagdoll(m_pickup.GetComponent<Rigidbody>());
                m_pickup.transform.SetParent(null);
                Vector3 throwforce = transform.forward * m_forwardThrowStrength;
                throwforce.y = m_upwardsThrowStrength;
                m_pickup.GetComponent<Rigidbody>().AddForce(throwforce);
                m_holdingObject = false;
            }
        }
	}

    void EnableRagdoll(Rigidbody rb)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
    }
    void DisableRagdoll(Rigidbody rb)
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
    }

    // Gets the closest pickup to the player in range
    GameObject GetClosestPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_pickUpRangeRadius, 1 << 11);

        if (hitColliders.Length == 0)
            return null;

        GameObject closestPickup = hitColliders[0].gameObject;
        Vector3 currentPos = transform.position;
        float minDist = 10000;

        for (int i = 1; i < hitColliders.Length; ++i)
        {
            float dist = Vector3.Distance(hitColliders[i].transform.position, currentPos);
            if (dist < minDist)
            {
                closestPickup = hitColliders[i].gameObject;
                minDist = dist;
            }
        }
        return closestPickup;
    }
}
