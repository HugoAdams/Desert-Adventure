using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlockAttack : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerStatusEffects>().FlattenedAttack();
            other.transform.GetComponent<PlayerController>().OnPlayerHit(2);
        }
    }
}
