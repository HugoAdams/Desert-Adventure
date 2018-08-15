using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusHit : MonoBehaviour {

    CactusMin body;
	void Start () {
        body = GetComponentInParent<CactusMin>();
	}

    private void OnTriggerEnter(Collider other)
    {
        body.OnEnemyHit(5, other.transform);
    }
}
