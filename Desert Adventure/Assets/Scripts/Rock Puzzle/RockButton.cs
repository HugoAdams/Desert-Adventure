using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockButton : MonoBehaviour {

    bool m_rockPlaced;

    public void TriggerRockPlaced(Quaternion rotation)
    {
        SoundEffectsPlayer.Instance.PlaySound("Tick");
        m_rockPlaced = true;
        GameObject rock = transform.Find("Rock").gameObject;
        rock.SetActive(true);
        //rock.transform.rotation = rotation;
        transform.parent.GetComponent<RockPuzzle>().CheckPuzzleComplete();
    }

    public bool RockIsPlaced()
    {
        return m_rockPlaced;
    }
}
