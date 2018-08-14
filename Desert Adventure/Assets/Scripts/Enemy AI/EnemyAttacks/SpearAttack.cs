using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("called");
            Vector3 attackDirection = (other.transform.position - GetComponent<CapsuleCollider>().transform.position);
            attackDirection.Normalize();
            other.gameObject.GetComponent<PlayerStatusEffects>().SpearedAttack(attackDirection);
            other.transform.GetComponent<PlayerController>().OnPlayerHit(1);
        }
    }
}
