using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour {

    public float m_pushForce = 500;
    Rigidbody m_rbody;

    private void Awake()
    {
        m_rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetButton("Vertical"))
        {
            m_rbody.AddForce(transform.up * m_pushForce * Time.deltaTime);
        }

        float horizontal = -Input.GetAxisRaw("Horizontal");
        transform.Rotate(new Vector3(0, 0, 45 * horizontal * Time.deltaTime));
    }
}
