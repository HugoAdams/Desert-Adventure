using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialScene : MonoBehaviour {

    [Header("items must be >= cacti")]
    public List<CactusMin> Cactilist;
    public List<Transform> ItemList;

    float startTimeToEnd = -1;

    PlayerMovement m_player = null;
    // Use this for initialization
    private void Start()
    {
        foreach(Transform item in ItemList)
        {
            item.gameObject.SetActive(false);
        }
    }

    void SpecialStart()
    {
        

        for (int i=0; i< Cactilist.Capacity; i++)
        {
            Cactilist[i].SpecRunToTransform(ItemList[i]);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Minus))
        {
            SpecialStart();
        }

        
	}

    void CheckIfItemsTaken()
    {
        bool alloff = true;
        foreach (Transform tr in ItemList)
        {
            if (tr.gameObject.activeSelf == true)
            {
                alloff = false;
                break;
            }
        }

        if(alloff == true)
        {
            startTimeToEnd = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hit = false, boat = false;
        if (other.GetComponentInParent<BoatMovement>())
        {
            hit = true;
            boat = true;
        }
        else if (other.GetComponent<PlayerMovement>())
        {
            hit = true;
        }

        if(hit == true)
        {
            BoatMovement bm = null;
            if(boat == true)
            {
                m_player = other.GetComponentInChildren<PlayerMovement>();
                bm = other.GetComponentInParent<BoatMovement>();
                bm.m_specialDismountBoat = true;
            }
            else
            {
                m_player = other.GetComponent<PlayerMovement>();
            }
            PlayerMove(false);
            DropBoatParts();

            //turn off this triggerbox
            GetComponent<BoxCollider>().enabled = false;
            //turnoff all other colliders
            transform.GetChild(0).gameObject.SetActive(false);


            //at the end of the scene turn off boat
        }

    }

    void PlayerMove(bool _move)
    {
        if(m_player == null)
        {
            return;
        }
        m_player.m_specialDontMove = _move;
        m_player.GetComponent<PlayerActions>().m_specialDontMove = _move;
        m_player.GetComponent<PlayerController>().m_specialDontMove = _move;
    }

    void DropBoatParts()
    {
        m_player.GetComponent<PlayerController>().LoseAllBoatParts();

        foreach (Transform item in ItemList)
        {
            item.gameObject.SetActive(true);
        }
    }
}
