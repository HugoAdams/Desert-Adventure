using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFaceColliders : MonoBehaviour {

    public CapsuleCollider bodyCollider = null;
    public CapsuleCollider noseCollider = null;
    public CapsuleCollider buttCollider = null;

    public void DisableAll()
    {
        bodyCollider.enabled = false;
        noseCollider.enabled = false;
        buttCollider.enabled = false;
    }
}
