using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPuzzle : MonoBehaviour {

    public List<RockButton> m_rockButtons;

    public GameObject m_RewardObject;

    private Vector3 m_rewardSpawnLocation;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            if (child.name == "RockButton")
                m_rockButtons.Add(child.gameObject.GetComponent<RockButton>());
            if (child.name == "SpawnLocation")
                m_rewardSpawnLocation = child.gameObject.transform.position;
        }
    }

    // Checks if all the rocks have been placed on all the buttons in the puzzle
    public void CheckPuzzleComplete()
    {
        foreach(RockButton button in m_rockButtons)
        {
            if (!button.RockIsPlaced())
                return;
        }

        Instantiate(m_RewardObject, m_rewardSpawnLocation, transform.rotation);
    }
}
