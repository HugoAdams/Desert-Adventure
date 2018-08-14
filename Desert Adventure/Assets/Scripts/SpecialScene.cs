﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialScene : MonoBehaviour {

    [Header("items must be >= cacti")]
    public List<CactusMin> Cactilist;
    public List<Transform> ItemList;

    float startTimeToEnd = -1;
	// Use this for initialization
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
}
