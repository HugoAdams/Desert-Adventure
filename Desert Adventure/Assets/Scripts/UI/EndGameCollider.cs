using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCollider : MonoBehaviour {

    public Transform m_endGamePrefab;
    private void OnTriggerEnter(Collider other)
    {
        Instantiate(m_endGamePrefab, m_endGamePrefab.position, m_endGamePrefab.rotation);
        Destroy(gameObject);
    }
}
