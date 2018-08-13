using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFaceColliders : MonoBehaviour {

    public CapsuleCollider bodyCollider = null;
    public CapsuleCollider noseCollider = null;
    public CapsuleCollider buttTrigger = null;

    public void DisableAll()
    {
        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }
        if (noseCollider != null)
        {
            noseCollider.enabled = false;
        }
        if (buttTrigger != null)
        {
            buttTrigger.enabled = false;
        }
    }
}
