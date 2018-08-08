using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour {

    public Transform m_target;

    private void LateUpdate()
    {
        if (m_target)
            transform.position = Vector3.Lerp(transform.position, m_target.position, 2 * Time.deltaTime);
    }
}
