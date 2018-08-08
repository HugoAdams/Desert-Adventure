using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlockAttack : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStatusEffects>().FlattenedAttack();
        }
    }
}
