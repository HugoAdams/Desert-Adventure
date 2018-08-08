using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Vector3 attackDirection = (other.transform.position - GetComponent<BoxCollider>().transform.position);
            attackDirection.Normalize();
            other.gameObject.GetComponent<PlayerStatusEffects>().SpearedAttack(attackDirection);
        }
    }
}
