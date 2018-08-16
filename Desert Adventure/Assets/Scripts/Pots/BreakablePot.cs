using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePot : MonoBehaviour {

    public GameObject m_heart;
    public GameObject m_breakingParticles;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(m_breakingParticles, transform.position, transform.rotation);
        if (Random.Range(0,2) == 0)
        {
           Instantiate(m_heart, transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }
        SoundEffectsPlayer.Instance.PlaySound("BreakPot");
        Destroy(gameObject);
    }
}
