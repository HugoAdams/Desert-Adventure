using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPuzzle : MonoBehaviour {

    public List<RockButton> m_rockButtons;

    public GameObject m_RewardObject;

    private Vector3 m_rewardSpawnLocation;

    private GameObject m_nextPuzzle;

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
        m_rewardSpawnLocation.y -= 5.0f;
        m_nextPuzzle = Instantiate(m_RewardObject, m_rewardSpawnLocation, transform.rotation);
        //m_rewardSpawnLocation.y += 20.0f;
        StartCoroutine(raisePuzzle());
    }

    // Raises the newly spawned puzzle out of the ground until it hits its spawn height
    IEnumerator raisePuzzle()
    {
       float desiredHeight = m_rewardSpawnLocation.y + 5.0f;
       Vector3 finalPosition = m_rewardSpawnLocation;
       finalPosition.y = desiredHeight;
       Transform puzzleTransform = m_nextPuzzle.transform;
       while (puzzleTransform.position.y < desiredHeight)
       {
           puzzleTransform.position = new Vector3(finalPosition.x + Random.Range(-0.05f, 0.05f), puzzleTransform.position.y + Random.Range(-0.05f, 0.05f), finalPosition.z + Random.Range(-0.05f, 0.05f));
           puzzleTransform.Translate(Vector3.up * Time.deltaTime * 0.8f, Space.World);
           yield return null;
       }
       puzzleTransform.position = finalPosition;
       yield return null;
    }
}
