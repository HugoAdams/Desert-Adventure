using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CagePuzzle : MonoBehaviour {

    public List<CactusMin> m_enemies;

    public Transform m_cageDoor;
    bool beat = false;
    float falltime = -1;
	// Update is called once per frame
	void Update ()
    {
        /*if(Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H");
            beat = true;
            falltime = Time.time;
        }*/
        if(beat == false)
        {
            CheckEnded();
        }
        else
        {
            DoorFall();
        }
	}

    void DoorFall()
    {
        if (Time.time - falltime < 13.0f)
        {
            m_cageDoor.transform.position += Vector3.down * 2.5f * Time.deltaTime;
        }
    }

    void CheckEnded()
    {
        bool check = true;
        for (int i = 0; i < m_enemies.Count; i++)
        {
            if(m_enemies[i] == null)
            {
                continue;
            }
            if (m_enemies[i].GetCurrentHealth() > 0)
            {
                check = false;
                break;
            }
        }

        beat = check;
        if(beat == true)
        {
            falltime = Time.time;
        }
    }
}
