using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour {

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * 700.0f);
        rb.AddForce(transform.forward * 600.0f);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * 50.0f);
        moveTowardsPlayer();
    }

    void moveTowardsPlayer()
    {
       Collider[] playerCollider = Physics.OverlapSphere(transform.position, 10, 1 << 15);

       if (playerCollider.Length == 0)
           return;

        transform.position = Vector3.MoveTowards(transform.position, playerCollider[0].transform.position, 6 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        other.GetComponent<PlayerController>().addHealth(1);
    }
}
