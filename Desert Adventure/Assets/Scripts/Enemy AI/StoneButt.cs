using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneButt : MonoBehaviour {

    StoneFaceMin body;
    private void Start()
    {
        body = GetComponentInParent<StoneFaceMin>();
    }

    private void OnTriggerEnter(Collider other)
    {
        body.OnEnemyHit(3, other.transform);
    }

}
