using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockButton : MonoBehaviour {

    public bool m_rockPlaced;

    public void TriggerRockPlaced(Quaternion rotation)
    {
        m_rockPlaced = true;
        GameObject rock = transform.Find("Rock").gameObject;
        rock.SetActive(true);
        rock.transform.rotation = rotation;
    }
}
