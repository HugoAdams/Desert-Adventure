using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePot : MonoBehaviour {

    public GameObject Heart;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        if(Random.Range(0,2) == 0)
        {
           Instantiate(Heart, transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        }
    }
}
