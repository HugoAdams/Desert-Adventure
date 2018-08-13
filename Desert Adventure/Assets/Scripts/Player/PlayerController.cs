using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerStats m_baseStats, m_currentStats;
    public Transform m_boatPrefab;

    bool m_onBoat;
    PlayerMovement m_movement;
    PlayerActions m_actions;

    private void Awake()
    {
        m_onBoat = false;
        m_movement = GetComponent<PlayerMovement>();
        m_actions = GetComponent<PlayerActions>();

        // FOR NOW, ALWAYS RESET PLAYER STATS TO DEFAULT
        m_currentStats.Reset(m_baseStats);
    }

    void Update ()
    {
        if (!m_onBoat)
        {
            GetOnBoatLogic();
        }
	}

    void GetOnBoatLogic()
    {
        if (Input.GetButtonDown("BoatMounting"))
        {
            // No boat base = can't get on boat!
            if (!m_currentStats.BoatBase)
            {
                EffectCanvas.Instance.InformText("MISSING: Boat Hull");
                return;
            }

            MountBoat();
        }
    }

    void MountBoat()
    {
        Camera.main.GetComponent<CameraController>().SetToBoatMode();
        GetComponent<CharacterController>().enabled = false;
        m_movement.MountBoat();
        m_actions.MountBoat();
        m_onBoat = true;

        Transform newBoat = Instantiate(m_boatPrefab, transform.position, transform.rotation);
        newBoat.GetComponent<BoatMovement>().Initialize(m_currentStats, this);
    }

    public void DismountBoat()
    {
        Camera.main.GetComponent<CameraController>().SetToPlayerMode();
        GetComponent<CharacterController>().enabled = true;
        m_movement.DismountBoat();
        m_actions.DismountBoat();
        m_onBoat = false;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Stand back up
    }
}
